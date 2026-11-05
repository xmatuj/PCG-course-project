using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using System.IO;

namespace Mayorov_Matvey_PRI_123_course
{
    public partial class Form1 : Form
    {
        // === Камера ===
        private float camX = 0.0f;
        private float camY = 8.0f;
        private float camZ = 20.0f;
        private float camAngleX = 0.0f;
        private float camAngleY = 0.0f;

        // === Мышь ===
        private float Mcoord_X = 0.0f;
        private float Mcoord_Y = 0.0f;
        private bool isMouseDown = false;

        // === Сцена ===
        private int currentScene = 0;   // 0 — 3D, 1 — 2D-векторная, 2 — 2D-растровая
        private readonly string[] sceneNames = { "3D (Зоопарк)", "2D-Векторная (Безье)", "2D-Растровая (Спрайт)" };

        // === Флаги режимов ===
        private bool animationEnabled = true;
        private bool lightingEnabled = true;

        // === Таймер и FPS ===
        private int frameCount = 0;
        private int fps = 0;
        private float time = 0.0f;

        // === Инициализация OpenGL ===
        private bool glInitialized = false;

        // Позиция и направление воробья
        private float sparrowX = 0.0f;
        private float sparrowY = 2.0f;
        private float sparrowZ = 0.0f;
        private float sparrowAngle = 0.0f;
        private float sparrowTargetAngle = 0.0f;

        // Спрайт воробья (атлас)
        private Bitmap sparrowAtlas;
        private int spriteFrameWidth = 64;
        private int spriteFrameHeight = 64;
        private int spriteFrameCount = 4;
        private int currentSpriteFrame = 0;
        private float spriteTimer = 0.0f;
        private const float SPRITE_FRAME_TIME = 0.15f;

        // ID текстур для каждого кадра
        private uint[] spriteTextures;

        // Спрайт с фильтром "Акварелизация"
        private uint[] spriteTexturesWatercolor;

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();

            AnT.MouseMove += AnT_MouseMove;
            AnT.MouseDown += AnT_MouseDown;
            AnT.MouseUp += AnT_MouseUp;

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        // === ЗАГРУЗКА ФОРМЫ ===
        private void Form1_Load(object sender, EventArgs e)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            Gl.glClearColor(0.5f, 0.7f, 1.0f, 1.0f);
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45.0, (double)AnT.Width / AnT.Height, 0.1, 200.0);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            glInitialized = true;

            // Генерируем фрактал "Бинарное дерево"
            fractalBranches.Clear();
            GenerateBinaryTree(AnT.Width / 2f, 0, 120, 0, 0);

            // Загрузка атласа спрайта воробья
            try
            {
                string atlasPath = Path.Combine(Application.StartupPath, "sparrow_atlas.png");
                if (File.Exists(atlasPath))
                {
                    sparrowAtlas = new Bitmap(atlasPath);
                    spriteFrameWidth = sparrowAtlas.Width / spriteFrameCount;
                    spriteFrameHeight = sparrowAtlas.Height;

                    // Загружаем текстуры для всех кадров сразу
                    spriteTextures = new uint[spriteFrameCount];
                    for (int i = 0; i < spriteFrameCount; i++)
                    {
                        Bitmap frame = GetSpriteFrame(sparrowAtlas, i);
                        Bitmap frameRGBA = new Bitmap(frame.Width, frame.Height, PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(frameRGBA)) { g.DrawImage(frame, 0, 0); }

                        spriteTextures[i] = LoadTextureFromBitmap(frameRGBA);

                        frame.Dispose();
                        frameRGBA.Dispose();
                    }

                    // Загружаем текстуры с фильтром "Акварелизация"
                    spriteTexturesWatercolor = new uint[spriteFrameCount];
                    for (int i = 0; i < spriteFrameCount; i++)
                    {
                        Bitmap frame = GetSpriteFrame(sparrowAtlas, i);
                        Bitmap frameRGBA = new Bitmap(frame.Width, frame.Height, PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(frameRGBA)) { g.DrawImage(frame, 0, 0); }

                        // Применяем фильтр "Акварелизация"
                        Bitmap filtered = ApplyWatercolorFilter(frameRGBA);
                        spriteTexturesWatercolor[i] = LoadTextureFromBitmap(filtered);

                        frame.Dispose();
                        frameRGBA.Dispose();
                        filtered.Dispose();
                    }
                }
                else
                {
                    MessageBox.Show("Файл sparrow_atlas.png не найден рядом с .exe", "Внимание");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки атласа: " + ex.Message);
            }

            RenderTimer.Start();
        }

        // === ТАЙМЕР ===
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            frameCount++;
            if (frameCount >= 60)
            {
                fps = frameCount;
                frameCount = 0;
                toolStripLabelFPS.Text = $"FPS: {fps}";
            }

            if (animationEnabled)
            {
                time += 0.05f;
                spriteTimer += 0.016f;
                if (spriteTimer >= SPRITE_FRAME_TIME)
                {
                    spriteTimer = 0.0f;
                    currentSpriteFrame = (currentSpriteFrame + 1) % spriteFrameCount;
                }
            }

            // Плавный поворот к целевому углу
            float diff = sparrowTargetAngle - sparrowAngle;
            if (diff > 180) diff -= 360;
            if (diff < -180) diff += 360;

            float rotateSpeed = 6.0f;
            if (Math.Abs(diff) < rotateSpeed)
                sparrowAngle = sparrowTargetAngle;
            else
                sparrowAngle += Math.Sign(diff) * rotateSpeed;

            if (sparrowAngle >= 360) sparrowAngle -= 360;
            if (sparrowAngle < 0) sparrowAngle += 360;

            Draw();
            toolStripLabelScene.Text = $"Сцена: {sceneNames[currentScene]}";
        }

        // === ОТРИСОВКА ===
        private void Draw()
        {
            if (!glInitialized) return;

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            Glu.gluLookAt(camX, camY, camZ, 0, 0, 0, 0, 1, 0);

            if (lightingEnabled)
            {
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glEnable(Gl.GL_LIGHT0);
                float[] lightPos = { 10.0f, 20.0f, 10.0f, 1.0f };
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPos);
            }
            else
            {
                Gl.glDisable(Gl.GL_LIGHTING);
            }

            if (currentScene == 0)
                Draw3DScene();
            else if (currentScene == 1)
                Draw2DVectorScene();
            else if (currentScene == 2)
                Draw2DRasterScene();

            Gl.glFlush();
            AnT.Invalidate();
        }

        // ==========================================
        // 3D СЦЕНА (Зоопарк)
        // ==========================================
        private void Draw3DScene()
        {
            // Земля
            Gl.glPushMatrix();
            Gl.glColor3f(0.2f, 0.8f, 0.2f);
            Gl.glTranslated(0, -0.1f, 0);
            Gl.glScaled(20, 0.1, 20);
            Glut.glutSolidCube(1.0f);
            Gl.glPopMatrix();

            // Воробей
            DrawSparrow();
        }

        private void DrawSparrow()
        {
            Gl.glPushMatrix();
            Gl.glTranslatef(sparrowX, sparrowY, sparrowZ);
            Gl.glRotatef(sparrowAngle, 0, 1, 0);

            float wingAngle = animationEnabled ? (float)Math.Sin(time * 10) * 25 : 0;
            float tailAngle = animationEnabled ? (float)Math.Sin(time * 6) * 10 : 0;

            // Тело
            Gl.glColor3f(0.55f, 0.38f, 0.20f);
            Gl.glPushMatrix();
            Gl.glScaled(0.45f, 0.40f, 0.70f);
            Glut.glutSolidSphere(1.0, 20, 20);
            Gl.glPopMatrix();

            // Голова
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 0.35f, -0.55f);
            Gl.glColor3f(0.60f, 0.45f, 0.25f);
            Gl.glScaled(0.32f, 0.32f, 0.32f);
            Glut.glutSolidSphere(1.0, 16, 16);
            Gl.glPopMatrix();

            // Шапочка
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 0.68f, -0.55f);
            Gl.glColor3f(0.20f, 0.12f, 0.06f);
            Gl.glScaled(0.30f, 0.10f, 0.30f);
            Glut.glutSolidSphere(1.0, 12, 12);
            Gl.glPopMatrix();

            // Глаза
            Gl.glColor3f(0.05f, 0.05f, 0.05f);
            Gl.glPushMatrix();
            Gl.glTranslatef(-0.26f, 0.40f, -0.75f);
            Gl.glScaled(0.07f, 0.07f, 0.07f);
            Glut.glutSolidSphere(1.0, 8, 8);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslatef(0.26f, 0.40f, -0.75f);
            Gl.glScaled(0.07f, 0.07f, 0.07f);
            Glut.glutSolidSphere(1.0, 8, 8);
            Gl.glPopMatrix();

            // Клюв
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 0.32f, -0.8f);
            Gl.glRotatef(180, 1, 0, 0);
            Gl.glColor3f(0.95f, 0.70f, 0.10f);
            Glut.glutSolidCone(0.08, 0.25, 10, 10);
            Gl.glPopMatrix();

            // Хвост
            Gl.glPushMatrix();
            Gl.glTranslatef(0.0f, 0.10f, 0.45f);
            Gl.glRotatef(20 + tailAngle, 1, 0, 0);
            Gl.glTranslatef(0.0f, 0.0f, 0.25f);
            Gl.glColor3f(0.35f, 0.22f, 0.10f);
            Gl.glScaled(0.30f, 0.08f, 0.50f);
            Glut.glutSolidCube(1.0f);
            Gl.glPopMatrix();

            // Лапки
            Gl.glColor3f(0.90f, 0.70f, 0.20f);
            Gl.glPushMatrix();
            Gl.glTranslatef(-0.12f, -0.40f, 0.05f);
            Gl.glRotatef(-90, 1, 0, 0);
            Glu.gluCylinder(Glu.gluNewQuadric(), 0.025, 0.025, 0.25, 8, 8);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslatef(0.12f, -0.40f, 0.05f);
            Gl.glRotatef(-90, 1, 0, 0);
            Glu.gluCylinder(Glu.gluNewQuadric(), 0.025, 0.025, 0.25, 8, 8);
            Gl.glPopMatrix();

            // Крылья
            Gl.glColor3f(0.45f, 0.30f, 0.15f);
            Gl.glPushMatrix();
            Gl.glTranslatef(-0.42f, 0.10f, 0.0f);
            Gl.glRotatef(wingAngle, 0, 0, 1);
            Gl.glScaled(0.35f, 0.10f, 0.45f);
            Glut.glutSolidSphere(1.0, 12, 12);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslatef(0.42f, 0.10f, 0.0f);
            Gl.glRotatef(-wingAngle, 0, 0, 1);
            Gl.glScaled(0.35f, 0.10f, 0.45f);
            Glut.glutSolidSphere(1.0, 12, 12);
            Gl.glPopMatrix();

            Gl.glPopMatrix();
        }

        // ==========================================
        // 2D-ВЕКТОРНАЯ СЦЕНА (Сплайн Безье)
        // ==========================================
        private void Draw2DVectorScene()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(0, AnT.Width, 0, AnT.Height);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            Gl.glDisable(Gl.GL_LIGHTING);

            // Фрактал
            Gl.glColor3f(0.0f, 0.39f, 0.0f);
            Gl.glLineWidth(2.0f);
            Gl.glBegin(Gl.GL_LINES);
            foreach (var (start, end) in fractalBranches)
            {
                Gl.glVertex2d(start.X, start.Y);
                Gl.glVertex2d(end.X, end.Y);
            }
            Gl.glEnd();
            Gl.glLineWidth(1.0f);

            // Кривая Безье
            Gl.glColor3f(0.8f, 0.2f, 0.2f);
            Gl.glLineWidth(3.0f);
            Gl.glBegin(Gl.GL_LINE_STRIP);

            float p0x = 100, p0y = 100;
            float p1x = 300, p1y = 500;
            float p2x = 600, p2y = 100;
            float p3x = 800, p3y = 500;

            for (float t = 0; t <= 1.0f; t += 0.01f)
            {
                float x = (float)(Math.Pow(1 - t, 3) * p0x + 3 * Math.Pow(1 - t, 2) * t * p1x + 3 * (1 - t) * Math.Pow(t, 2) * p2x + Math.Pow(t, 3) * p3x);
                float y = (float)(Math.Pow(1 - t, 3) * p0y + 3 * Math.Pow(1 - t, 2) * t * p1y + 3 * (1 - t) * Math.Pow(t, 2) * p2y + Math.Pow(t, 3) * p3y);
                Gl.glVertex2d(x, y);
            }
            Gl.glEnd();

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }

        // ==========================================
        // 2D-РАСТРОВАЯ СЦЕНА (Только спрайт воробья)
        // ==========================================
        private void Draw2DRasterScene()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(0, AnT.Width, 0, AnT.Height);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glColor3f(1, 1, 1);

            // Левый спрайт — обычный
            if (spriteTextures != null && spriteTextures.Length > 0)
            {
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, spriteTextures[currentSpriteFrame]);

                float sprX = AnT.Width / 4.0f - spriteFrameWidth / 2.0f;
                float sprY = (AnT.Height - spriteFrameHeight) / 2.0f;

                Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2f(0, 0); Gl.glVertex2f(sprX, sprY);
                Gl.glTexCoord2f(1, 0); Gl.glVertex2f(sprX + spriteFrameWidth, sprY);
                Gl.glTexCoord2f(1, 1); Gl.glVertex2f(sprX + spriteFrameWidth, sprY + spriteFrameHeight);
                Gl.glTexCoord2f(0, 1); Gl.glVertex2f(sprX, sprY + spriteFrameHeight);
                Gl.glEnd();
            }

            // Правый спрайт — с фильтром "Акварелизация"
            if (spriteTexturesWatercolor != null && spriteTexturesWatercolor.Length > 0)
            {
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, spriteTexturesWatercolor[currentSpriteFrame]);

                float sprX = 3.0f * AnT.Width / 4.0f - spriteFrameWidth / 2.0f;
                float sprY = (AnT.Height - spriteFrameHeight) / 2.0f;

                Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2f(0, 0); Gl.glVertex2f(sprX, sprY);
                Gl.glTexCoord2f(1, 0); Gl.glVertex2f(sprX + spriteFrameWidth, sprY);
                Gl.glTexCoord2f(1, 1); Gl.glVertex2f(sprX + spriteFrameWidth, sprY + spriteFrameHeight);
                Gl.glTexCoord2f(0, 1); Gl.glVertex2f(sprX, sprY + spriteFrameHeight);
                Gl.glEnd();
            }

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }

        // ==========================================
        // ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
        // ==========================================

        // Фильтр "Акварелизация"
        private Bitmap ApplyWatercolorFilter(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);

            for (int x = 1; x < original.Width - 1; x++)
            {
                for (int y = 1; y < original.Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = original.GetPixel(x + i, y + j);
                            r += pixel.R;
                            g += pixel.G;
                            b += pixel.B;
                        }
                    }
                    r /= 9; g /= 9; b /= 9;

                    r = Math.Min(255, r + 20);
                    g = Math.Min(255, g + 20);
                    b = Math.Min(255, b + 20);

                    // Сохраняем альфа-канал из исходного пикселя
                    Color orig = original.GetPixel(x, y);
                    result.SetPixel(x, y, Color.FromArgb(orig.A, r, g, b));
                }
            }
            return result;
        }

        // Извлекает один кадр из горизонтального атласа
        private Bitmap GetSpriteFrame(Bitmap atlas, int frameIndex)
        {
            Rectangle srcRect = new Rectangle(
                frameIndex * spriteFrameWidth, 0,
                spriteFrameWidth, spriteFrameHeight);

            Bitmap frame = new Bitmap(spriteFrameWidth, spriteFrameHeight);
            using (Graphics g = Graphics.FromImage(frame))
            {
                g.DrawImage(atlas, new Rectangle(0, 0, spriteFrameWidth, spriteFrameHeight),
                            srcRect, GraphicsUnit.Pixel);
            }
            return frame;
        }

        // Загрузка текстуры из Bitmap в OpenGL
        private uint LoadTextureFromBitmap(Bitmap bmp)
        {
            uint texId = 0;
            Gl.glGenTextures(1, out texId);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texId);

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

            byte[] pixels = BitmapToByteArrayRGBA(bmp);

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA,
                            bmp.Width, bmp.Height, 0,
                            Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);

            return texId;
        }

        // Преобразование Bitmap в массив байтов RGBA (с переворотом по Y)
        private byte[] BitmapToByteArrayRGBA(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            byte[] result = new byte[width * height * 4];

            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;
                    int stride = data.Stride;

                    for (int y = 0; y < height; y++)
                    {
                        int srcY = height - 1 - y;
                        byte* row = ptr + srcY * stride;

                        for (int x = 0; x < width; x++)
                        {
                            int idx = (y * width + x) * 4;
                            result[idx + 0] = row[x * 4 + 2]; // R
                            result[idx + 1] = row[x * 4 + 1]; // G
                            result[idx + 2] = row[x * 4 + 0]; // B
                            result[idx + 3] = row[x * 4 + 3]; // A
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            return result;
        }

        // Генерация бинарного дерева
        private List<(PointF start, PointF end)> fractalBranches = new List<(PointF, PointF)>();

        private void GenerateBinaryTree(float x, float y, float length, float angleDeg, int iteration)
        {
            if (iteration > 7) return;

            float rad = (float)(angleDeg * Math.PI / 180);
            float endX = x + length * (float)Math.Sin(rad);
            float endY = y + length * (float)Math.Cos(rad);

            fractalBranches.Add((new PointF(x, y), new PointF(endX, endY)));

            float newLength = length * 0.72f;
            GenerateBinaryTree(endX, endY, newLength, angleDeg - 25, iteration + 1);
            GenerateBinaryTree(endX, endY, newLength, angleDeg + 25, iteration + 1);
        }

        // === УПРАВЛЕНИЕ ===
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            float speed = 0.5f;
            switch (e.KeyCode)
            {
                case Keys.W: sparrowZ -= speed; sparrowTargetAngle = 0f; break;
                case Keys.S: sparrowZ += speed; sparrowTargetAngle = 180f; break;
                case Keys.A: sparrowX -= speed; sparrowTargetAngle = 90f; break;
                case Keys.D: sparrowX += speed; sparrowTargetAngle = -90f; break;
                case Keys.Space: sparrowY += speed; break;
                case Keys.ShiftKey: sparrowY -= speed; break;
            }
        }

        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            Mcoord_X = e.X;
            Mcoord_Y = e.Y;
            toolStripLabelMouseX.Text = $"Mouse X: {Mcoord_X:F0}";
            toolStripLabelMouseY.Text = $"Mouse Y: {Mcoord_Y:F0}";
        }

        private void AnT_MouseDown(object sender, MouseEventArgs e) { isMouseDown = true; }
        private void AnT_MouseUp(object sender, MouseEventArgs e) { isMouseDown = false; }

        // === МЕНЮ И КНОПКИ ===
        private void выходToolStripMenuItem_Click(object sender, EventArgs e) { Application.Exit(); }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Курсовой проект\nТема: Воссоздание сцены из мультфильма «Где обедал воробей»\nАвтор: Майоров М. С., гр. ПРИ-123", "О программе");
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e) { currentScene = 0; }
        private void векторнаяToolStripMenuItem_Click(object sender, EventArgs e) { currentScene = 1; }
        private void растроваяToolStripMenuItem_Click(object sender, EventArgs e) { currentScene = 2; }
        private void toolStripButton3D_Click(object sender, EventArgs e) { currentScene = 0; }
        private void toolStripButton2DVec_Click(object sender, EventArgs e) { currentScene = 1; }
        private void toolStripButton2DRast_Click(object sender, EventArgs e) { currentScene = 2; }
        private void toolStripButtonAnim_Click(object sender, EventArgs e) { animationEnabled = !animationEnabled; }
        private void toolStripButtonLight_Click(object sender, EventArgs e) { lightingEnabled = !lightingEnabled; }
        private void toolStripButtonExit_Click(object sender, EventArgs e) { Application.Exit(); }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
    }
}