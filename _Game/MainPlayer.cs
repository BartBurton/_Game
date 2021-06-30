using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static System.Math;
using static _Game.Obstacle;

namespace _Game
{
    static class MainPlayer
    {
        //Вывод картинки
        static Texture texture;
        static Sprite SBird;
        static IntRect SIntRect = new IntRect(381, 187, BIRD_W, BIRD_H);

        //Анимация падения, взмахов и поворотов
        static float Gravity = 9.8f;
        static bool Up = false;//Флаг для указания взлета или падения
        static float LastDownCoordinate;//Последняя координата по Y перед взлетом
        static float AnimationCounter = 0.05f;//Скорость анимации
        const byte SHIFT = 26;//Для анимации взмахов
        public static Vector2f Pos;//Для изменения позиции

        //Поиск коллизий
        static float PointTopRight;//Координата по Y правой верхней точки
        static float PointDownRight;//Координата по Y правой нижней точки
        static float PointDown;//Координата по Y нижней точки
        static float Rotation;//Угол поворота спрайта
        const float GRADE = (float)(PI / 180); //Константа для перевода в радианы
        public const byte BIRD_W = 17; //Ширина спрайта
        const byte BIRD_H = 12; //Высота спрайта
        static readonly float BIRD_D = (float)Sqrt((BIRD_W * BIRD_W) + (BIRD_H * BIRD_H));//Диагональ

        //Геймплей
        public static bool Die { get; private set; } = false;
        public static bool OnGround { get; private set; } = false;

        //Звуки
        public static SoundBuffer WingBuffer = new SoundBuffer("wing.wav");
        public static SoundBuffer DieBuffer = new SoundBuffer("die.wav");
        public static SoundBuffer FastWingBuffer = new SoundBuffer("Jump.wav");
        public static SoundBuffer FastDieBuffer = new SoundBuffer("Ponc.wav");
        public static Sound Wing = new Sound();
        public static Sound SDie = new Sound();

        static MainPlayer()
        {
            texture = new Texture(PlayGame.FileSprites);

            SBird = new Sprite(texture, new IntRect(381, 187, BIRD_W, 64));
            SBird.TextureRect = SIntRect;
            SBird.Scale = new Vector2f(3, 3);

            Pos.Y = PlayGame.HEIGHT / 2;

            PointsCoordinate();
        }

        //Полет во время игры
        public static void DrawBird()
        {
            Drop();
            if (!Die)
            {
                WingsFlap();
                Swing();
                Collision();
            }
            PlayGame.Window.Draw(SBird);
        }

        //Полет без падения(в меню)
        public static void DrawBirdNotDrop()
        {
            WingsFlap();
            PlayGame.Window.Draw(SBird);
        }

        //Сброс состояния игрока
        public static void ClearBird()
        {
            Pos.X = (PlayGame.WIGTH / 2) - (BIRD_W * 3);
            Pos.Y = PlayGame.HEIGHT / 2;
            SBird.Position = Pos;
            SBird.Rotation = 0;
            PointsCoordinate("PDR");
            OnGround = false;
            Die = false;
        }

        //Анимация полета
        static void WingsFlap()
        {
            if (AnimationCounter > 0.58)
            {
                SBird.TextureRect = SIntRect;
                SIntRect.Top += (SIntRect.Top != 239) ? SHIFT : -2 * SHIFT;
                AnimationCounter = 0;
            }
            AnimationCounter += 0.05f;
        }

        //При нажатии на клавишу, несколько циклов игры, происходит 
        //измение координаты по Y и наклона носа вверх
        static void Swing()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space) || Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                if (SBird.Position.Y < 70)
                {
                    PointsCoordinate("PTR");
                    if (PointTopRight < 50) return;
                }

                if (Up) return;
                Up = true;
                Wing.Play();
                LastDownCoordinate = Pos.Y;
            }
        }

        //Увеличениие высоты на 70 пикселей
        //подъем носа вверх
        static void Flight()
        {
            Pos.Y -= 7f;

            if (SBird.Rotation > 0)
                SBird.Rotation -= 8;
            else if (SBird.Rotation != -10) SBird.Rotation -= 10 - Abs(SBird.Rotation);

            //Перестать подниматься при изменении высоты на 70
            if ((LastDownCoordinate - Pos.Y) > 70)
            {
                Up = false;
                return;
            }
            SBird.Position = Pos;
        }

        //Уменьшение высоты и наклон вниз
        static void Drop()
        {
            //Если мы поднимаемся, то не выполнять падение
            if (Up)
            {
                Flight();
                return;
            }

            if (SBird.Position.Y > PlayGame.HEIGHT - 120)
            {
                PointsCoordinate("PDR");
                if (PointDownRight >= PlayGame.HEIGHT - 85)
                {
                    Die = true;
                    OnGround = true;
                    return;
                }
            }

            //Падение
            if (SBird.Rotation <= -7)
            {
                SBird.Rotation += 1f;
            }
            else if (SBird.Rotation <= 10)
            {
                SBird.Rotation += 1.1f;
                Pos.Y += Gravity / 1.3f;
            }
            else if (SBird.Rotation <= 80)
            {
                SBird.Rotation += 3f;
                Pos.Y += Gravity / 1f;
            }
            else
                Pos.Y += Gravity / 0.95f;


            SBird.Position = Pos;
        }

        //Проверка на сотлкновение с препядствием
        static void Collision()
        {
            if (PosCenterObs == 0 || Die) return;
            if (ClashOccured(PosCenterObs - 190, PosCenterObs))
                Die = true;
        }
        //Поиск точки задевшей препядствие
        static bool ClashOccured(float yTop, float yDown)
        {
            PointsCoordinate();

            return (SBird.Position.Y < yTop || SBird.Position.Y > yDown || PointTopRight < yTop || PointTopRight > yDown
            || PointDownRight < yTop || PointDownRight > yDown || PointDown < yTop || PointDown > yDown);
        }
        //Поиск координат вершин спрайта по Y
        static void PointsCoordinate()
        {
            PointTopRight = SBird.Position.Y;
            PointDownRight = PointTopRight;
            PointDown = PointTopRight;
            Rotation = (SBird.Rotation < 0) ? 360 - SBird.Rotation : SBird.Rotation;
            PointTopRight += BIRD_W * (float)Sin(Rotation * GRADE);
            PointDownRight += BIRD_D * (float)Sin((Rotation + 35.5f) * GRADE);
            PointDown += BIRD_H * (float)Sin((Rotation + 90) * GRADE);
        }

        //Поиск координат вершин спрайта по Y
        //PTR - правая верхняя
        //PDR - правая нижняя
        //PD - нижняя
        static void PointsCoordinate(string Dot)
        {
            switch (Dot)
            {
                case ("PTR"):
                    {
                        Rotation = (SBird.Rotation < 0) ? 360 - SBird.Rotation : SBird.Rotation;
                        PointTopRight = SBird.Position.Y;
                        PointTopRight += BIRD_W * (float)Sin(Rotation * GRADE);
                    }
                    break;
                case ("PDR"):
                    {
                        Rotation = (SBird.Rotation < 0) ? 360 - SBird.Rotation : SBird.Rotation;
                        PointDownRight = SBird.Position.Y;
                        PointDownRight += BIRD_D * (float)Sin((Rotation + 35.5f) * GRADE);
                    }
                    break;
                case ("PD"):
                    {
                        Rotation = (SBird.Rotation < 0) ? 360 - SBird.Rotation : SBird.Rotation;
                        PointDown = SBird.Position.Y;
                        PointDown += BIRD_H * (float)Sin((Rotation + 90) * GRADE);
                    }
                    break;
            }

        }
    }
}