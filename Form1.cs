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
        private readonly string[] sceneNames = { "3D (Зоопарк)", "2D-Векторная (Безье)", "2D-Растровая (Акварель)" };

        // === Флаги режимов ===
        private bool animationEnabled = true;
        private bool lightingEnabled = true;
        private bool soundEnabled = true;

        // === Таймер и FPS ===
        private int frameCount = 0;
        private int fps = 0;
        private float time = 0.0f; // Для анимации

        // === Многопоточность (ЛР 3) ===
        private object locker = new object();
        private Thread physicsThread;
        private bool physicsRunning = false;

        // === Инициализация OpenGL ===
        private bool glInitialized = false;

        // === Объекты сцены (ЛР 9 - Геометрические преобразования) ===
        // Позиция воробья
        private float sparrowX = 0.0f;
        private float sparrowY = 2.0f;
        private float sparrowZ = 0.0f;
        private float sparrowAngle = 0.0f;

        // Для 2D-растровой сцены (ЛР 7, акварелизация)
        private Bitmap originalImage;
        private Bitmap filteredImage;

        // Спрайт воробья (атлас)
        private Bitmap sparrowAtlas;
        private int spriteFrameWidth = 64;   // ширина одного кадра
        private int spriteFrameHeight = 64;  // высота одного кадра
        private int spriteFrameCount = 4;    // количество кадров в атласе
        private int currentSpriteFrame = 0;  // текущий кадр
        private float spriteTimer = 0.0f;    // таймер анимации
        private const float SPRITE_FRAME_TIME = 0.15f; // время на 1 кадр (сек)

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

            // Генерируем фрактал "Бинарное дерево" (ЛР 10, вариант 10)
            fractalBranches.Clear();
            GenerateBinaryTree(AnT.Width / 2f, 0, 120, 0, 0);

            // Создаем тестовое изображение для 2D-растровой сцены
            originalImage = CreateTestImage();
            filteredImage = ApplyWatercolorFilter(originalImage);

            // Загрузка атласа спрайта воробья
            try
            {
                string atlasPath = Path.Combine(Application.StartupPath, "sparrow_atlas.png");
                if (File.Exists(atlasPath))
                {
                    sparrowAtlas = new Bitmap(atlasPath);

                    // Автоматически вычисляем размер кадра, если атлас горизонтальный
                    spriteFrameWidth = sparrowAtlas.Width / spriteFrameCount;
                    spriteFrameHeight = sparrowAtlas.Height;
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

                // Анимация спрайта
                spriteTimer += 0.016f; // ~16 мс на кадр (60 FPS)
                if (spriteTimer >= SPRITE_FRAME_TIME)
                {
                    spriteTimer = 0.0f;
                    currentSpriteFrame = (currentSpriteFrame + 1) % spriteFrameCount;
                }
            }

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
            {
                Draw3DScene();
            }
            else if (currentScene == 1)
            {
                Draw2DVectorScene();
            }
            else if (currentScene == 2)
            {
                Draw2DRasterScene();
            }

            Gl.glFlush();
            AnT.Invalidate();
        }

        // ==========================================
        // 3D СЦЕНА (Зоопарк)
        // ==========================================
        private void Draw3DScene()
        {
            // 1. Земля (ЛР 9 - масштабирование)
            Gl.glPushMatrix();
            Gl.glColor3f(0.2f, 0.8f, 0.2f);
            Gl.glTranslated(0, -0.1f, 0);
            Gl.glScaled(20, 0.1, 20);
            Glut.glutSolidCube(1.0f);
            Gl.glPopMatrix();

            // 3. Клетка льва (ЛР 9 - перенос)
            Gl.glPushMatrix();
            Gl.glTranslated(-5, 1, -5);
            DrawCage();
            Gl.glPopMatrix();

            // 4. Воробей (Управляемый объект, ЛР 9)
            Gl.glPushMatrix();
            Gl.glTranslatef(sparrowX, sparrowY, sparrowZ);
            Gl.glRotatef(sparrowAngle, 0, 1, 0);

            float wingAngle = (float)Math.Sin(time * 10) * 20;

            // Тело
            Gl.glColor3f(0.6f, 0.4f, 0.2f);
            Gl.glPushMatrix();
            Gl.glScaled(0.5, 0.4, 0.7);
            Glut.glutSolidSphere(1.0, 16, 16);
            Gl.glPopMatrix();

            // Крылья
            Gl.glColor3f(0.5f, 0.3f, 0.1f);
            Gl.glPushMatrix();
            Gl.glTranslated(-0.3, 0.1, 0);
            Gl.glRotated(wingAngle, 0, 0, 1);
            Gl.glScaled(0.1, 0.3, 0.5);
            Glut.glutSolidSphere(1.0, 8, 8);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(0.3, 0.1, 0);
            Gl.glRotated(-wingAngle, 0, 0, 1);
            Gl.glScaled(0.1, 0.3, 0.5);
            Glut.glutSolidSphere(1.0, 8, 8);
            Gl.glPopMatrix();

            Gl.glPopMatrix();

            // 5. Поверхность вращения на основе кривой Безье (ЛР 8, вариант 10)
            // Поилка для моржа (или просто декоративный объект)
            Gl.glPushMatrix();
            Gl.glTranslated(5, 1, -5);
            Gl.glColor3f(0.7f, 0.7f, 0.8f);
            DrawBezierRevolvedSurface();
            Gl.glPopMatrix();
        }

        private void DrawCage()
        {
            Gl.glColor3f(0.5f, 0.5f, 0.5f);
            Gl.glLineWidth(2);
            Gl.glBegin(Gl.GL_LINES);
            for (int i = -2; i <= 2; i += 2)
            {
                Gl.glVertex3d(i, 0, -2); Gl.glVertex3d(i, 4, -2);
                Gl.glVertex3d(i, 0, 2); Gl.glVertex3d(i, 4, 2);
                Gl.glVertex3d(-2, 0, i); Gl.glVertex3d(-2, 4, i);
                Gl.glVertex3d(2, 0, i); Gl.glVertex3d(2, 4, i);
            }
            Gl.glEnd();
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

            // ==========================================
            // 1. Фрактал "Бинарное дерево" (вариант 10)
            // ==========================================
            Gl.glColor3f(0.0f, 0.39f, 0.0f); // тёмно-зелёный (вариант 10)
            Gl.glLineWidth(2.0f);
            Gl.glBegin(Gl.GL_LINES);
            foreach (var (start, end) in fractalBranches)
            {
                Gl.glVertex2d(start.X, start.Y);
                Gl.glVertex2d(end.X, end.Y);
            }
            Gl.glEnd();
            Gl.glLineWidth(1.0f);

            // ==========================================
            // 2. Кривая Безье (ЛР 8)
            // ==========================================
            Gl.glColor3f(0.8f, 0.2f, 0.2f); // красный, чтобы отличать от фрактала
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
        // 2D-РАСТРОВАЯ СЦЕНА (Спрайт по центру)
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

            if (sparrowAtlas != null)
            {
                Bitmap currentFrame = GetSpriteFrame(sparrowAtlas, currentSpriteFrame);
                // Преобразуем в RGBA (с альфа-каналом)
                Bitmap frameRGBA = new Bitmap(currentFrame.Width, currentFrame.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(frameRGBA))
                {
                    g.DrawImage(currentFrame, 0, 0);
                }

                byte[] spritePixels = BitmapToByteArrayRGBA(frameRGBA);

                int drawX = (AnT.Width - spriteFrameWidth) / 2;
                int drawY = (AnT.Height - spriteFrameHeight) / 2;

                Gl.glRasterPos2i(drawX, drawY);
                Gl.glDrawPixels(frameRGBA.Width, frameRGBA.Height,
                                Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, spritePixels);

                currentFrame.Dispose();
                frameRGBA.Dispose();
            }

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
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

        // Преобразование Bitmap в массив байтов RGB (с переворотом по Y)
        private byte[] BitmapToByteArray(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            byte[] result = new byte[width * height * 3];

            // Блокируем биты для быстрого доступа
            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;
                    int stride = data.Stride;

                    // OpenGL ожидает изображение снизу вверх, поэтому идем с конца
                    for (int y = 0; y < height; y++)
                    {
                        // Строка в исходном изображении (сверху вниз)
                        int srcY = height - 1 - y;
                        byte* row = ptr + srcY * stride;

                        for (int x = 0; x < width; x++)
                        {
                            // Format24bppRgb хранит BGR, а не RGB
                            result[(y * width + x) * 3 + 0] = row[x * 3 + 2]; // R
                            result[(y * width + x) * 3 + 1] = row[x * 3 + 1]; // G
                            result[(y * width + x) * 3 + 2] = row[x * 3 + 0]; // B
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

        // Преобразование Bitmap в массив байтов RGBA (с альфа-каналом)
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
                            // Format32bppArgb хранит BGRA
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

        // ==========================================
        // ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
        // ==========================================

        // Поверхность вращения на основе кривой Безье (ЛР 8, вариант 10)
        private void DrawBezierRevolvedSurface()
        {
            int segments = 20; // Количество сегментов по окружности
            int curveSteps = 20; // Количество точек на кривой

            // Контрольные точки кривой Безье (профиль поилки)
            float[] px = { 0.0f, 0.5f, 1.0f, 0.8f };
            float[] py = { 0.0f, 1.0f, 1.5f, 2.0f };

            // Генерируем массив точек кривой Безье
            List<PointF> curvePoints = new List<PointF>();
            for (int i = 0; i <= curveSteps; i++)
            {
                float t = (float)i / curveSteps;
                float x = (float)(Math.Pow(1 - t, 3) * px[0] + 3 * Math.Pow(1 - t, 2) * t * px[1] + 3 * (1 - t) * Math.Pow(t, 2) * px[2] + Math.Pow(t, 3) * px[3]);
                float y = (float)(Math.Pow(1 - t, 3) * py[0] + 3 * Math.Pow(1 - t, 2) * t * py[1] + 3 * (1 - t) * Math.Pow(t, 2) * py[2] + Math.Pow(t, 3) * py[3]);
                curvePoints.Add(new PointF(x, y));
            }

            // Строим поверхность вращения
            Gl.glBegin(Gl.GL_QUAD_STRIP);
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)(2 * Math.PI * i / segments);
                float cosA = (float)Math.Cos(angle);
                float sinA = (float)Math.Sin(angle);

                foreach (var p in curvePoints)
                {
                    Gl.glVertex3f(p.X * cosA, p.Y, p.X * sinA);
                }
            }
            Gl.glEnd();
        }

        // Генерация бинарного дерева (вариант 10) — для 2D-векторной сцены
        private List<(PointF start, PointF end)> fractalBranches = new List<(PointF, PointF)>();

        private void GenerateBinaryTree(float x, float y, float length, float angleDeg, int iteration)
        {
            if (iteration > 7) return; // глубина рекурсии

            // Конец текущей ветви
            float rad = (float)(angleDeg * Math.PI / 180);
            float endX = x + length * (float)Math.Sin(rad);
            float endY = y + length * (float)Math.Cos(rad);

            // Сохраняем ветвь как отрезок
            fractalBranches.Add((new PointF(x, y), new PointF(endX, endY)));

            // Две ветви отклоняются на ±25°
            float newLength = length * 0.72f;
            GenerateBinaryTree(endX, endY, newLength, angleDeg - 25, iteration + 1);
            GenerateBinaryTree(endX, endY, newLength, angleDeg + 25, iteration + 1);
        }

        // Создание тестового изображения (для акварелизации)
        private Bitmap CreateTestImage()
        {
            Bitmap bmp = new Bitmap(AnT.Width, AnT.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightBlue);
                // Рисуем простой пейзаж для теста
                g.FillRectangle(Brushes.Green, 0, AnT.Height / 2, AnT.Width, AnT.Height / 2);
                g.FillEllipse(Brushes.Yellow, AnT.Width / 2 - 50, 50, 100, 100); // Солнце
                g.FillRectangle(Brushes.Brown, 100, 300, 50, 200); // Ствол дерева
                g.FillEllipse(Brushes.DarkGreen, 50, 200, 150, 150); // Крона
            }
            return bmp;
        }

        // Фильтр "Акварелизация" (ЛР 7, вариант 10)
        private Bitmap ApplyWatercolorFilter(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height);

            // Упрощенная акварелизация: размытие + усиление контуров
            // В реальности это сложный алгоритм, но для демонстрации сойдет
            for (int x = 1; x < original.Width - 1; x++)
            {
                for (int y = 1; y < original.Height - 1; y++)
                {
                    // Берем среднее значение цвета (размытие)
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

                    // Для акварели немного повышаем яркость
                    r = Math.Min(255, r + 20);
                    g = Math.Min(255, g + 20);
                    b = Math.Min(255, b + 20);

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return result;
        }

        // === УПРАВЛЕНИЕ ===
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            float speed = 0.5f;
            switch (e.KeyCode)
            {
                case Keys.W: sparrowZ -= speed; break;
                case Keys.S: sparrowZ += speed; break;
                case Keys.A: sparrowX -= speed; sparrowAngle = -90; break;
                case Keys.D: sparrowX += speed; sparrowAngle = 90; break;
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

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}