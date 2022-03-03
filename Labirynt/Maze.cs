using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labirynt
{
    /**
     * Klasa komórki labiryntu 
     */
    class Cell
    {
        /**
         * Struktura ściany labiryntu
         */
        public struct Wall
        {
            public Label wall;
            public bool active;
        }

        /**Tablica ścian */
        public Wall[] walls;
        /**grafika komórki */
        public Label cell;
        /**zmienna wykorzystywana przy generowaniu labiryntu */
        public bool visited = false;

        /**zmienna przechowująca pozycję x komórki */
        public int x;
        /**zmienna przechowująca pozycję y komórki */
        public int y;
        /**szerokość komórki*/
        private int w;
        /**grubość ścian */
        public static int wT = 4;

        /**
         *Konstruktor - przypisanie wartości odpowiednich zmiennych, oraz
         *zainicjalizowanie ścian
         */
        public Cell(int x, int y, int w, int px, int py)
        {
            this.x = x;
            this.y = y;
            this.w = w;

            cell = new Label();
            cell.BackColor = Color.LightPink;
            cell.Size = new Size(w, w);
            cell.Visible = false;
            cell.Location = new Point(px + x * w, py + y * w);
            cell.BringToFront();

            walls = new Wall[4];

            initWalls();
        }

        /**
         * Funkcja inicjalizująca ściany
         */
        private void initWalls()
        {
            for (int i = 0; i < 4; i++)
            {
                walls[i].wall = new Label();
                walls[i].wall.BackColor = Color.Black;
                walls[i].wall.Visible = true;
                walls[i].wall.Size = new Size(0, 0);
                walls[i].wall.BringToFront();

                walls[i].active = true;
            }
        }

        /**
        * Funkcja wyświetlająca komórkę wraz ze ścianami
        */
        public void show()
        {
            cell.Visible = true;

            var lx = cell.Location.X;
            var ly = cell.Location.Y;

            //Górna ściana
            walls[0].wall.Location = new Point(lx, ly);
            walls[0].wall.Size = new Size(w + wT, wT);

            if (walls[0].active) walls[0].wall.Visible = true;
            else walls[0].wall.Visible = false;


            //Prawa ściana
            walls[1].wall.Location = new Point(lx + w, ly);
            walls[1].wall.Size = new Size(wT, w + wT);

            if (walls[1].active) walls[1].wall.Visible = true;
            else walls[1].wall.Visible = false;


            //Dolna ściana
            walls[2].wall.Location = new Point(lx, ly + w);
            walls[2].wall.Size = new Size(w + wT, wT);

            if (walls[2].active) walls[2].wall.Visible = true;
            else walls[2].wall.Visible = false;


            //Lewa ściana
            walls[3].wall.Location = new Point(lx, ly);
            walls[3].wall.Size = new Size(wT, w + wT);

            if (walls[3].active) walls[3].wall.Visible = true;
            else walls[3].wall.Visible = false;
        }

    }

    /**
     * Klasa stosu wykorzystywana podczas generacji labiryntu
     */
    class Stack
    {
        /**Tablica stosu */
        private Cell[] stack;
        /**Zmienna do przechowywania elementu znajdującego się na górze stosu */
        public int top;

        /**
         * Konstruktor inicjalizujący stos
         */
        public Stack(int size)
        {
            top = 0;
            stack = new Cell[size];
        }

        /**
         * Funkcja dodawania elementu do stosu
         */
        public void push(Cell cell)
        {
            if (top < stack.Length) stack[top++] = cell;

            else
            {
                Cell[] temp = new Cell[top + 10];
                for (int i = 0; i < stack.Length; i++) temp[i] = stack[i];

                stack = temp;
                stack[top++] = cell;
            }
        }
        /**
         * Funkcja usuwania elementu do stosu.
         * Zwraca usunięty element
         */
        public Cell pop()
        {
            if (top > 0) return stack[--top];

            else return null;
        }
    }

    /**
     * Główna klasa WindowsForms
     * */
    public partial class Maze : Form
    {
        /**Zmienna przechowująca stan generacji labiryntu */
        private bool done = false;
        /**Zmienna przechowywująca stan gry */
        private bool gameOver = false;
        /**Zmienna szerokości okna aplikacji*/
        private int form_width = 1280;
        /**Zmienna wysokości okna aplikacji*/
        private int form_height = 1024;
        /**Zmienna rozmiaru labiryntu*/
        private int width, height;
        /**Zmienna pozycji X labiryntu*/
        private int posX;
        /**Zmienna pozycji Y labiryntu*/
        private int posY;
        /**Zmienna przechowujące indeks wejścia do  labiryntu*/
        private int start_pos;
        /**Zmienna przechowujące indeks wyjścia z  labiryntu*/
        private int end_pos;
        /**Zmienna przechowująca ilość kolumn w labiryncie*/
        private int cols;
        /**Zmienna przechowująca ilość rzędów w labiryncie*/
        private int rows;
        /**Aktualny poziom*/
        private int level = 1;
        /**Maksymalny poziom*/
        private int max_level = 10;
        /**Szerokość komórki labiryntu */
        private int w = 90;
        /**Zegar*/
        private System.Timers.Timer t;
        /**Początkowa wartośc czasu: minuty*/
        private int m = 1;
        /**Początkowa wartośc czasu: sekundy*/
        private int s = 30;
        /**Sekundy*/
        private int ts = 60;
        /**Wynik*/
        private int sc = 0;
        /**Siatka labiryntu*/
        private Cell[] grid;
        /**Obiekt przechowujący aktualną komórkę*/
        private Cell current;
        /**Obiekt przechowujący następną komórkę*/
        private Cell next;
        /**Obiekt gracza*/
        private Cell player;
        /**Obiekt wyjścia z labiryntu*/
        private Cell goal;
        /**Przyciski wejścia i wyjścia*/
        private Button start, exit;
        /**Obiekt pola tekstu wyniki*/
        private Label score;
        /**Obiekt pola tekstu komunikatu*/
        private Label alert;
        /**Obiekt pola tekstu czasu*/
        private Label time;
        /**Obiekt pola tekstu poziomu*/
        private Label currentLvl;
        /**Obiekt pola tekstu końca gry*/
        private Label gameOverAlert;
        /**Obiekt pola tekstu tytułu*/
        private Label title;
        /**Obiekt stosu*/
        private Stack stack;

        /**
         * Konstruktor aplikacji - ustawia ogólny wygląd, oraz inicjalizuje poszczególne elementy
         * i ustawia ich pozycje.
         */
        public Maze()
        {
            InitializeComponent();

            this.Size = new Size(form_width, form_height);
            this.BackColor = Color.White;
            this.AutoSize = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(form_width, form_height);
            this.MinimumSize = new System.Drawing.Size(form_width, form_height);


            width = form_width / 2;
            height = form_height / 2;

            posX = form_width / 2 - width / 2;
            posY = form_height / 2 - height / 2;

            initGrid();

            initButtons();
            initText();

            t = new System.Timers.Timer();
            t.Interval = 1000;//1s
            t.Elapsed += OnTimeEvent;

            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyDown);
            this.DoubleBuffered = true;
        }

        /**
        * Funkcja odpowiedzialna za wczytanie wyników.
        * Zwraca tablicę z wynikami.
        */
        private int[] readScore()
        {
            string[] text = System.IO.File.ReadAllLines("scores.txt");
            int[] val = new int[text.Count()];

            for (int i = 1; i < text.Count(); i++)
            {
                val[i] = int.Parse(text[i]);
            }

            Array.Sort(val);

            return val;
        }

        /**
         * Funkcja odpowiedzialna za zapis wyniku
         */
        private void addScore()
        {
            System.IO.File.AppendAllText("scores.txt", "" + sc + "\n");
        }

        /**
         * Funkcja inicjalizująca obiekty tekstu
         */
        private void initText()
        {
            title = new Label();
            score = new Label();
            alert = new Label();
            time = new Label();
            gameOverAlert = new Label();
            currentLvl = new Label();

            score.Text = "Aktualny wynik: 0000";
            score.Location = new Point(posX + 30, form_height / 2 + height / 2 - 40);
            score.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            score.Size = new Size(score.Text.Length * 25, 30);
            score.Visible = false;

            time.Text = "Pozostały czas: 00:00";
            time.Location = new Point(posX + 30, form_height / 2 - height / 2 - 40);
            time.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            time.Size = new Size(score.Text.Length * 25, 30);
            time.Visible = false;

            title.Text = "ŚWIATEŁKO W TUNELU";
            title.Font = new System.Drawing.Font("Microsoft Sans Serif", 44F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            title.AutoSize = true;
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Location = new Point(start.Location.X - 5 * title.Size.Width / 2, start.Location.Y - 200);
            title.Visible = true;

            currentLvl.Text = "Aktualny poziom: 1";
            currentLvl.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            currentLvl.Size = new Size(score.Text.Length * 25, 30);
            currentLvl.Location = new Point(posX + 30, form_height / 2 - height / 2 - 80);
            currentLvl.Visible = false;

            gameOverAlert.Text = "KONIEC GRY";
            gameOverAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            gameOverAlert.Size = new Size(500, 500);
            gameOverAlert.Location = new Point(form_width / 2 - 250, form_height / 2 - 250 - 200);
            gameOverAlert.Visible = false;

            alert.Text = "GENEROWANIE LABIRYNTU";
            alert.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            alert.Size = new Size(700, 100);
            alert.Location = new Point(posX, form_height / 2 - height / 2 - 60);
            alert.Visible = false;

            Controls.Add(title);
            Controls.Add(score);
            Controls.Add(time);
            Controls.Add(currentLvl);
            Controls.Add(alert);
            Controls.Add(gameOverAlert);
        }

        /**
         * Funkcja wywołująca się z każdym krokiem zegara.
         * Aktualizuję tekst pozostałego czasu
         */
        private void OnTimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                s--;
                if (m <= 0)
                {
                    m = 0;
                }
                if (s < 0)
                {
                    s = 0;
                    if (m > 0)
                    {
                        s = 59;
                        m--;
                    }
                    else
                    {
                        gameOver = true;
                    }
                }

                if (gameOver)
                {
                    t.Stop();
                    draw();
                }
                time.Text = "Pozostały czas: " + string.Format("{0}:{1}", m.ToString().PadLeft(2, '0'), s.ToString().PadLeft(2, '0'));
            }));
        }

        /**
         * Funkcja inicjalizująca przyciski
         */
        private void initButtons()
        {
            start = new Button();
            start.TabStop = false;
            start.Text = "START";
            start.FlatAppearance.BorderSize = 3;
            start.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            start.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            start.Size = new Size(150, 50);
            start.BackColor = Color.Pink;
            start.Location = new Point(form_width / 2 - start.Size.Width / 2, form_height / 2 - start.Size.Height / 2);
            start.Click += new System.EventHandler(startClick);
            start.CausesValidation = false;
            Controls.Add(start);

            exit = new Button();
            exit.Text = "WYJDŹ";
            exit.TabStop = false;
            exit.FlatAppearance.BorderSize = 3;
            exit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            exit.Size = new Size(start.Text.Length * 30, start.Text.Length * 10);
            exit.BackColor = Color.Pink;
            exit.Location = new Point(start.Location.X, start.Location.Y + 5 * start.Size.Height / 4);
            exit.Click += new System.EventHandler(exitClick);
            exit.CausesValidation = false;
            Controls.Add(exit);
        }

        /**
         * Funkcja wywołująca się przy wciśnięciu przycisku start
         */
        private void startClick(object sender, EventArgs e)
        {
            start.Visible = false;
            title.Visible = false;
            exit.Visible = false;

            readScore();
            draw();
            start.Enabled = false;
        }

        /**
         * Funkcja wywołująca się przy wciśnięciu przycisku wyjście
         */
        private void exitClick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /**
         * Funkcja inicjalizująca gracza
         */
        private void initPlayer()
        {
            int sx = w - 2 * Cell.wT, sy = w - 2 * Cell.wT;

            int px = player.cell.Location.X, py = player.cell.Location.Y;
            int gx = goal.cell.Location.X, gy = goal.cell.Location.Y;

            player = new Cell(grid[start_pos].x, grid[start_pos].y, w, posX, posY);
            goal = new Cell(grid[end_pos].x, grid[end_pos].y, w, posX, posY);

            Controls.Add(goal.cell);
            goal.cell.BackColor = Color.Yellow;
            goal.cell.Size = new Size(sx, sy);
            goal.cell.Location = new Point(gx + 3 * Cell.wT / 2, gy + 3 * Cell.wT / 2);
            goal.cell.Visible = true;
            goal.cell.BringToFront();

            Controls.Add(player.cell);
            player.cell.BackColor = Color.HotPink;
            player.cell.Size = new Size(sx, sy);
            player.cell.Location = new Point(px + 3 * Cell.wT / 2, py + 3 * Cell.wT / 2);
            player.cell.Visible = true;
            player.cell.BringToFront();
        }

        /**
         * Funkcja rysująca labirynt, oraz wyświetlająca tabelę wyników
         */
        public void draw()
        {
            if (!gameOver)
            {
                if (level <= max_level)
                {
                    alert.Visible = true;
                    alert.SendToBack();
                    time.Visible = false;
                    currentLvl.Visible = false;
                    score.Visible = false;
                    score.Location = new Point(posX + 30, form_height / 2 + height / 2);

                    grid[start_pos].walls[0].active = false;
                    grid[end_pos].walls[2].active = false;

                    // Implementacja algorytmu
                    //https://en.wikipedia.org/wiki/Maze_generation_algorithm#Recursive_implementation
                    do
                    {
                        wait(1);
                        for (int i = 0; i < grid.Count(); i++)
                        {
                            grid[i].show();
                        }

                        grid[index(current.x, current.y)] = current;
                        current.visited = true;
                        next = checkNeighbours(current.x, current.y);

                        if (next != null)
                        {

                            next.visited = true;

                            stack.push(current);

                            removeWalls(current, next);

                            current = grid[index(next.x, next.y)];
                        }
                        else if (stack.top > 0)
                        {
                            current = stack.pop();
                        }

                    } while (stack.top > 0);

                    initPlayer();

                    grid[end_pos].cell.Visible = true;
                    grid[start_pos].cell.Visible = true;

                    alert.Visible = false;
                    score.Visible = true;
                    currentLvl.Visible = true;
                    time.Visible = true;
                    done = true;
                    t.Start();
                }
                else
                {
                    gameOver = true;
                    draw();
                }

                exit.Location = new Point(form_width - exit.Size.Width - 10, form_height - exit.Size.Height - 10);
                exit.Visible = true;
            }
            else
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        Controls.Remove(grid[index(x, y)].cell);
                        for (int i = 0; i < 4; i++)
                        {
                            Controls.Remove(grid[index(x, y)].walls[i].wall);
                        }
                    }
                }


                Controls.Remove(player.cell);
                Controls.Remove(goal.cell);


                gameOverAlert.Visible = true;
                time.Visible = false;
                score.Visible = false;
                alert.Visible = false;
                currentLvl.Visible = false;
                gameOverAlert.TextAlign = ContentAlignment.MiddleCenter;
                gameOverAlert.Text = "KONIEC GRY\nUdało Ci się przejść " + (level - 1) + " labiryntów!\n" + "Twój wynik to: " + sc;

                //tabela wyników
                calcualteScore();
                addScore();

                int[] scores = readScore();
                Label[] table = new Label[5];
                Label text = new Label();

                text.Text = "TWOJE NAJLEPSZE WYNIKI";
                text.Location = new Point(form_width/2-text.Size.Width*3/2, gameOverAlert.Location.Y + 500);
                text.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                text.Size = new Size(text.Text.Length * 25, 30);
                text.BringToFront();
                text.Visible = true;
                Controls.Add(text);

                int ind = -1;
                for (int i = 0; i < 5; i++)
                {
                    if (sc == scores[scores.Count() - 1 - i]) ind = i;
                }

                for (int i = 0; i < 5; i++)
                {
                    table[i] = new Label();
                    if (ind != -1 && i == ind) table[i].ForeColor = Color.HotPink;
                    if (i < scores.Count())
                    {
                        table[i].Text = "" + (i + 1) + " " + scores[scores.Count() - 1 - i];
                        table[i].Location = new Point(form_width/2-table[i].Size.Width/2, text.Location.Y + text.Size.Height + 30 * i);
                        table[i].TextAlign = ContentAlignment.MiddleCenter;
                        table[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                        table[i].Visible = true;
                        table[i].BringToFront();
                        Controls.Add(table[i]);
                    }
                }
                exit.Visible = true;

                if (5 > scores.Count()) exit.Location = new Point(form_width/2-exit.Size.Width/2, table[scores.Count() - 1].Location.Y + 30);
                else exit.Location = new Point(form_width/2 - exit.Size.Width / 2, table[4].Location.Y + 30);

            }

            
        }

        /**
         * Funkcja wykorzystywana podczas generowania labiryntu.
         * Usuwa ściany między kolejnymi komórkami w celu utworzenia ścieżki
         */
        private void removeWalls(Cell a, Cell b)
        {
            int x = a.x - b.x;
            int y = a.y - b.y;

            if (x == 1)
            {
                a.walls[3].active = false;
                b.walls[1].active = false;
            }
            else if (x == -1)
            {
                a.walls[1].active = false;
                b.walls[3].active = false;
            }
            else if (y == 1)
            {
                a.walls[0].active = false;
                b.walls[2].active = false;
            }
            else if (y == -1)
            {
                a.walls[2].active = false;
                b.walls[0].active = false;
            }

        }

        /**
         * Funkcja wykorzystywana podczas geneorwania labirytnu.
         * Sprawdza sąsiadujące komórki i wybiera losową z nich.
         * Zwraca wybraną komórkę
         */
        private Cell checkNeighbours(int x, int y)
        {
            Cell[] neighbours = new Cell[4];

            Cell top = current;
            Cell right = current;
            Cell bottom = current;
            Cell left = current;

            if (y == 0 && x == 0) // lewy górny rog
            {
                right = grid[index(x + 1, y)];
                bottom = grid[index(x, y + 1)];
            }
            else if (y == 0 && x == cols - 1) // prawy górny rog
            {
                bottom = grid[index(x, y + 1)];
                left = grid[index(x - 1, y)];
            }
            else if (y == rows - 1 && x == cols - 1) // prawy dolny rog
            {
                top = grid[index(x, y - 1)];
                left = grid[index(x - 1, y)];
            }
            else if (y == rows - 1 && x == 0) // lewy dolny rog
            {
                top = grid[index(x, y - 1)];
                right = grid[index(x + 1, y)];
            }
            else if (x == 0 && y > 0 && y < rows) // lewa sciana
            {
                top = grid[index(x, y - 1)];
                right = grid[index(x + 1, y)];
                bottom = grid[index(x, y + 1)];
            }
            else if (x == cols - 1 && y > 0 && y < rows) // prawa sciana
            {
                top = grid[index(x, y - 1)];
                bottom = grid[index(x, y + 1)];
                left = grid[index(x - 1, y)];
            }
            else if (y == 0 && x > 0 && x < cols) // górna sciana
            {
                right = grid[index(x + 1, y)];
                bottom = grid[index(x, y + 1)];
                left = grid[index(x - 1, y)];
            }
            else if (y == rows - 1 && x > 0 && x < cols) // dolna sciana
            {
                top = grid[index(x, y - 1)];
                right = grid[index(x + 1, y)];
                left = grid[index(x - 1, y)];
            }
            else
            {
                top = grid[index(x, y - 1)];
                right = grid[index(x + 1, y)];
                bottom = grid[index(x, y + 1)];
                left = grid[index(x - 1, y)];
            }

            if (!top.visited) neighbours[0] = top;
            if (!right.visited) neighbours[1] = right;
            if (!bottom.visited) neighbours[2] = bottom;
            if (!left.visited) neighbours[3] = left;

            int r = GenerateRandomInt(0, 4);
            int count = 0;
            for (int i = 0; i < 4; i++) if (neighbours[i] != null) count++;

            if (count > 0)
            {
                do
                {
                    r = GenerateRandomInt(0, 4);
                } while (neighbours[r] == null);

                return neighbours[r];
            }
            else return null;

        }

        /**
         * Funkcja zwracająca indeks jednowymiarowej tablicy.
         * Przyjmuje indeksy tablicy dwuwymiarowej
         */
        private int index(int x, int y)
        {
            return x + y * cols;
        }

        /**
         * Funkcja odpowiedzialna za ruch gracza
         */
        private void keyDown(object sender, KeyEventArgs e)
        {
            var px = player.cell.Location.X;
            var py = player.cell.Location.Y;

            this.Focus();
            if (!gameOver)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        if (!gameOver && done && player.y != 0 && !grid[index(player.x, player.y - 1)].walls[2].active)
                        {
                            player.cell.Location = new Point(px, py - w);
                            player.y--;
                        }
                        break;
                    case Keys.D:
                        if (!gameOver && done && player.x != cols - 1 && !grid[index(player.x + 1, player.y)].walls[3].active)
                        {
                            player.cell.Location = new Point(px + w, py);
                            player.x++;
                        }
                        break;
                    case Keys.S:
                        if (!gameOver && done && player.y != rows - 1 && !grid[index(player.x, player.y + 1)].walls[0].active)
                        {
                            player.cell.Location = new Point(px, py + w);
                            player.y++;
                        }
                        break;
                    case Keys.A:
                        if (!gameOver && done && player.x != 0 && !grid[index(player.x - 1, player.y)].walls[1].active)
                        {
                            player.cell.Location = new Point(px - w, py);
                            player.x--;
                        }
                        break;
                }
                if (player.x == goal.x && player.y == goal.y)
                {
                    t.Stop();
                    player.cell.Visible = false;
                    goal.cell.Visible = false;

                    for (int y = 0; y < rows; y++)
                    {
                        for (int x = 0; x < cols; x++)
                        {
                            Controls.Remove(grid[index(x, y)].cell);
                            for (int i = 0; i < 4; i++)
                            {
                                Controls.Remove(grid[index(x, y)].walls[i].wall);
                            }
                        }
                    }
                    calcualteScore();

                    Controls.Remove(player.cell);
                    Controls.Remove(goal.cell);

                    level++;
                    w = 90 - level * 4;
                    currentLvl.Text = "Aktualny poziom: " + level;

                    initGrid();
                    draw();

                }
            }
        }

        /**
         * Funkcja kalkulująca wynik gracza
         * aktualizuje pole tekstowe wyniku.
         */
        private void calcualteScore()
        {
            if (!gameOver)
            {
                sc += (m * level / max_level - s / (s - ts) * level / max_level) * level * 100 + ts - s;
                ts = s;
            }
            if (sc % 1000 == sc)
            {
                if (sc % 100 == sc)
                {
                    score.Text = "Aktualny wynik: 00" + sc;
                }
                else
                {
                    score.Text = "Aktualny wynik: 0" + sc;
                }
            }
            else
            {
                score.Text = "Aktualny wynik: " + sc;
            }
        }

        /**
         * Funkcja wykorzystywana podczas generowania labiryntu.
         * Inicjalizuje, składającą się ze komórek siatkę labiryntu.
         */
        private void initGrid()
        {
            cols = width / w;
            rows = height / w;

            stack = new Stack(cols * rows);

            start_pos = index(cols / 2, 0);
            end_pos = index(cols / 2, rows - 1);

            done = false;
            grid = new Cell[cols * rows];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    grid[index(x, y)] = new Cell(x, y, w, posX, posY);
                    for (int j = 0; j < 4; j++) Controls.Add(grid[index(x, y)].walls[j].wall);
                    Controls.Add(grid[index(x, y)].cell);
                }
            }


            current = grid[start_pos];
            player = grid[start_pos];
            goal = grid[end_pos];
        }

        /**
         * Funkcja zwracająca losową wartośc całkowitą z podanego przedziału
         */
        private int GenerateRandomInt(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(max);
        }

        /**
         * Funkcja usypiająca program na określony czas
         */
        private void wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}
