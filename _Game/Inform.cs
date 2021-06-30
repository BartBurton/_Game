using System;
using System.IO;
using SFML.Graphics;
using SFML.System;
using System.Numerics;


namespace _Game
{
    //Начальное меню
    static class Menu
    {
        //Название игры
        static Texture texC = new Texture(PlayGame.FileSprites, new IntRect(152, 200, 89, 24));
        static Sprite Called;
        //Клавиша для начала игры
        static Texture texR = new Texture(PlayGame.FileSprites, new IntRect(384, 73, 34, 9));
        static Sprite Run;
        //Надпись на клавише
        static Texture texGR = new Texture(PlayGame.FileSprites, new IntRect(254, 71, 92, 25));
        static Sprite GetReady;

        //Клафиша для быстрой игры
        static Texture texF = new Texture(PlayGame.FileSprites, new IntRect(359, 71, 10, 11));
        static Sprite Fast;

        //Для измеения прозрачности клавиши
        static Color Clarity;
        static bool Definition = true;

        static Menu()
        {
            Called = new Sprite(texC);
            Called.Scale = new Vector2f(4, 4);
            Called.Position = new Vector2f((PlayGame.WIGTH / 2) - ((89 * 4) / 2) - 25, (PlayGame.HEIGHT / 2) - ((24 * 4) / 2) - 200);

            Run = new Sprite(texR);
            Run.Scale = new Vector2f(5, 5);
            Run.Position = new Vector2f((PlayGame.WIGTH / 2) - ((34 * 5) / 2) - 25, ((PlayGame.HEIGHT / 2) - (9 * 5) / 2) + 100);

            texGR.Smooth = true;
            GetReady = new Sprite(texGR);
            GetReady.Position = new Vector2f((PlayGame.WIGTH / 2) - (92 / 2) - 25, ((PlayGame.HEIGHT / 2) - (25 / 2) + 100));

            Fast = new Sprite(texF);
            Fast.Scale = new Vector2f(5, 5);
            Fast.Position = new Vector2f((PlayGame.WIGTH / 2) - ((10 * 5) / 2) - 25, ((PlayGame.HEIGHT / 2) - (11 * 5) / 2));
        }

        public static void DrawMenu()
        {
            PlayGame.Window.Draw(Called);
            PlayGame.Window.Draw(Run); 
            PlayGame.Window.Draw(GetReady);
            PlayGame.Window.Draw(Fast);
            Blinking();
        }

        //Мигание пробела
        private static void Blinking()
        {
            Clarity = GetReady.Color;
            if(Definition)
            {
                Clarity.A -= 5;
                if (Clarity.A == 100) Definition = false;
                GetReady.Color = Clarity;

                Clarity = Run.Color;
                Clarity.A -= 5;
                Run.Color = Clarity;

                Clarity = Fast.Color;
                Clarity.A -= 5;
                Fast.Color = Clarity;
            }
            else
            {
                Clarity.A += 5;
                if (Clarity.A == 255) Definition = true;
                GetReady.Color = Clarity;

                Clarity = Run.Color;
                Clarity.A += 5;
                Run.Color = Clarity;

                Clarity = Fast.Color;
                Clarity.A += 5;
                Fast.Color = Clarity;
            }
        }
    }

    //Вывод результата
    class Result
    {
        protected Vector2f Pos;
        protected Sprite[] Numbers = new Sprite[10];
        //Вывод кол-ва пройденных препядствий
        protected void DrawResult(Sprite[] numbers, BigInteger count, ushort widthNumber, Vector2f pos)
        {
            if(count == 0)
            {
                numbers[0].Position = pos;
                PlayGame.Window.Draw(numbers[0]);
                return;
            }

            while (count != 0)
            {
                numbers[(int)(count % 10)].Position = pos;
                PlayGame.Window.Draw(numbers[(int)(count % 10)]);
                pos.X -= widthNumber;

                count /= 10;
            }

        }
    }

    //Вывод результата во время игры
    class ResultInGame : Result
    {
        const byte WIGTH = 12;
        const byte HEIGHT = 18;

        public ResultInGame()
        {
            Numbers[0] = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(254, 98, WIGTH, HEIGHT)));
            Numbers[1] = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(238, 80, WIGTH, HEIGHT)));
            for (int i = 2; i < 6; i++)
                Numbers[i] = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(325 + ((i - 2) * 14), 148, WIGTH, HEIGHT)));
            for (int i = 6; i < 10; i++)
                Numbers[i] = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(325 + ((i - 6) * 14), 172, WIGTH, HEIGHT)));

            Numbers[0].Scale = new Vector2f(2, 2);
            for (int i = 1; i < Numbers.Length; i++)
            {
                Numbers[i].Scale = Numbers[0].Scale;
            }

            Pos = new Vector2f(PlayGame.WIGTH / 5, PlayGame.HEIGHT / 12);
        }

        public void DrawCount(BigInteger Count)
        {
            DrawResult(Numbers, Count, (WIGTH * 2) + 5, Pos);
        }
    }

    //Вывод конечного результата
    class ResultInGameOver : Result
    {
        const byte WIGTH = 6;
        const byte HEIGHT = 7;

        //Окно с результатом
        Sprite ResultWindow;

        //Соответствующая результату медаль
        Sprite Medal;
        Sprite Bronze = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(214, 102, 22, 22)));
        Sprite Silver = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(214, 78, 22, 22)));
        Sprite Gold = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(384, 154, 22, 22)));
        Sprite Platinum = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(384, 130, 22, 22)));

        //Блики на медальке
        Sprite Blicks = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(290, 190, 3, 3)));
        Vector2f PosBlicks;
        Random random = new Random();
        byte speedAnimation = 100;

        //Новый рекорд
        Sprite New;
        BigInteger Best;
        string[] Bests;

        public ResultInGameOver()
        {
            ResultWindow = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(260, 195, 113, 57)));
            ResultWindow.Scale = new Vector2f(3, 3);
            ResultWindow.Position = new Vector2f((PlayGame.WIGTH / 2) - (ResultWindow.Scale.X * ResultWindow.TextureRect.Width) / 2,
                (PlayGame.HEIGHT / 2) - (ResultWindow.Scale.Y * ResultWindow.TextureRect.Height) / 2);

            for (int i = 0; i < Numbers.Length; i++)
            {
                if (i % 2 == 0)
                    Numbers[i] = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(279 + ((i / 2) * 10), 171, WIGTH, HEIGHT)));
                else
                    Numbers[i] = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(279 + (((i - 1) / 2) * 10), 180, WIGTH, HEIGHT)));
                Numbers[i].Scale = ResultWindow.Scale;
            }

            Pos = new Vector2f(ResultWindow.Position.X + (95 * 3), ResultWindow.Position.Y + (16 * 3));

            Bronze.Scale = ResultWindow.Scale;
            Bronze.Position = new Vector2f(ResultWindow.Position.X + (13 * 3), ResultWindow.Position.Y + (21 * 3));

            Silver.Position = Bronze.Position;
            Silver.Scale = Bronze.Scale;

            Gold.Position = Bronze.Position;
            Gold.Scale = Bronze.Scale;

            Platinum.Position = Bronze.Position;
            Platinum.Scale = Bronze.Scale;

            Blicks.Scale = Bronze.Scale;

            New = new Sprite(new Texture(PlayGame.FileSprites, new IntRect(214, 126, 16, 7)));
            New.Scale = ResultWindow.Scale;
            New.Position = new Vector2f(ResultWindow.Position.X + (64 * 3), ResultWindow.Position.Y + (30 * 3));


            ReadFromFileBest();
        }

        void ReadFromFileBest()
        {
            Bests = File.ReadAllLines("Best.txt");
            if (Bests.Length == 0) Bests = new string[2] { "-1", "-1" };
            else if (Bests.Length > 2) Bests = new string[2] { "-1", "-1" };
            else if (Bests.Length == 2)
            {
                if (Bests[0] == "") Bests[0] = "-1";
                if (Bests[1] == "") Bests[1] = "-1";
            }
            else if (Bests.Length == 1)
            {
                if (Bests[0] == "") Bests = new string[2] {"-1", "-1"};
                else Bests = new string[2] { Bests[0], "-1" };
            }
        }

        //Выбор медали
        private void ChooseMedal(ref BigInteger Count)
        {
            if (Count < 10)
                Medal = Bronze;
            else if (Count > 10 && Count < 15)
                Medal = Silver;
            else if (Count > 15 && Count < 20)
                Medal = Gold;
            else
                Medal = Platinum;
        }

        public void DrawResultWindow(BigInteger Count, bool f = true)
        {
            PlayGame.Window.Draw(ResultWindow);

            Best = BigInteger.Parse((f) ? Bests[1] : Bests[0]);

            ChooseMedal(ref Count);
            if (Count > Best)
            {
                Best = Count;

                if (!f) Bests[0] = Best.ToString();
                else Bests[1] = Best.ToString();

                File.WriteAllLines("Best.txt", Bests);
            }
            if (Count == Best) PlayGame.Window.Draw(New);

            PlayGame.Window.Draw(Medal);
            SpeedAnimationBliks();
            PlayGame.Window.Draw(Blicks);

            DrawResult(Numbers, Count, (WIGTH * 3) + 1, Pos);
            Pos.Y += 23 * 3;
            DrawResult(Numbers, Best, (WIGTH * 3) + 1, Pos);
            Pos.Y -= 23 * 3;
        }

        //Анимация бликов на медали
        private void SpeedAnimationBliks()
        {
            if (speedAnimation == 100)
            {
                PosBlicks.X = random.Next((int)Medal.Position.X, (int)Medal.Position.X + Medal.TextureRect.Width * (int)Medal.Scale.X);
                PosBlicks.Y = random.Next((int)Medal.Position.Y, (int)Medal.Position.Y + Medal.TextureRect.Height);
                Blicks.Position = PosBlicks;
                speedAnimation = 0;
            }
            speedAnimation++;
        }
    }
}

