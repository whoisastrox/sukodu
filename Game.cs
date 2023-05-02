namespace Sukodu
{
    public partial class Game : Form
    {
        public Game()
        {
            InitializeComponent();
        }

        static List<List<SudokuButton>> Board = new List<List<SudokuButton>>(9);
        public class SudokuButton : Button
        {
            public int number = 0;
            public int solution;
            public SudokuButton(int input) : base()
            {
                this.Click += this.clickHandler;
                this.solution = input;
                this.Font = new Font("Cascadia Mono", 14, FontStyle.Bold);
                this.Text = ""; //Testing purposes only: Convert.ToString(this.solution);
            }
            protected void clickHandler(object sender, EventArgs e)
            {
                number++;
                if (number == 10) number = 1;
                this.Text = Convert.ToString(number);
            }
            public int getNumber()
            {
                return Convert.ToInt16(this.number);
            }

            public void setNumber(int n)
            {
                this.Text = Convert.ToString(n);
            }

            public void HideButton()
            {
                this.Text = "";
            }

            public void setHint()
            {
                this.Text = Convert.ToString(this.solution);
                this.Enabled = false;
                return;
            }

        }
        public class CheckerButton : Button
        {
            public CheckerButton() : base()
            {
                this.Location = new Point(750, 25);
                this.Size = new Size(125, 150);
                this.Text = "Clicca per controllare!";
                this.Click += this.clickHandler;
                this.FontHeight = 12;
                this.Font = new Font("Cascadia Mono", 14, FontStyle.Bold);
            }
            protected void clickHandler(object sender, EventArgs e)
            {
                if (CheckSolution() == true)
                {
                    MessageBox.Show("Hai vinto!");
                }
                else
                {
                    MessageBox.Show("Ritenta...");
                }
            }
        }
        public class HintButton : Button
        {
            public HintButton() : base()
            {
                this.Location = new Point(750, 175);
                this.Size = new Size(125, 150);
                this.Font = new Font("Cascadia Mono", 14, FontStyle.Bold);
                this.Text = "Clicca per ricevere un indizio!";
                this.Click += this.clickHandler;
                this.FontHeight = 12;
            }
            public void clickHandler(object sender, EventArgs e)
            {
                Game.createHint();
            }
        }

        public class Solution
        {
            public List<List<int>> Answer = new List<List<int>>(9);
            public Solution()
            {
                for (int i = 0; i < 9; i++)
                {
                    List<int> line = new List<int>(9);
                    for (int j = 0; j < 9; j++)
                    {
                        int num = 1;
                        line.Add(num);
                    }
                    Answer.Add(line);
                }
                GenerateSolution();
                return;
            }
            public List<int> CircularShift(int shift, List<int> line)
            {

                List<int> shifted = new List<int>(9);

                for (int i = 0; i < 9; i++)
                {
                    int position = i - shift;
                    if (position < 0) { position += 9; }

                    shifted.Add(line[position % 9]);
                }

                return shifted;
            }
            public void GenerateSolution()
            {

                Random rnd = new Random();

                int n = 9;

                List<int> nums = new List<int>(9);
                for (int j = 1; j < 10; j++)
                {
                    nums.Add(j);
                }

                Action<List<int>> shuffle = nums =>
                {
                    while (n > 1)
                    {
                        n--;
                        int k = rnd.Next(n + 1);
                        int value = nums[k];
                        nums[k] = nums[n];
                        nums[n] = value;
                    }
                };

                shuffle(nums);

                for (int i = 0; i < 9; i++)
                {
                    if (i == 0) Answer[i] = nums;
                    if (i > 0)
                    {
                        if (i % 3 == 0)
                        {
                            Answer[i] = CircularShift(1, Answer[i - 1]);
                        }
                        if (i % 3 == 1)
                        {
                            Answer[i] = CircularShift(3, Answer[i - 1]);
                        }
                        if (i % 3 == 2)
                        {
                            Answer[i] = CircularShift(3, Answer[i - 1]);
                        }
                    }
                }

            }
            public int getAnswer(int i, int j)
            {
                return Answer[i][j];
            }

        }

        public static Solution Sol = new Solution();

        public static bool CheckSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Board[i][j].getNumber() != Sol.getAnswer(i, j))
                    {
                        //Testing purposes only!
                        //MessageBox.Show("errore in "+i+ " - " +j);
                        //MessageBox.Show("" + Board[i][j].getNumber() + " - " + Sol.getAnswer(i, j));
                        return false;
                    }
                }
            }
            return true;
        }

        public static void createHint()
        {
            if (givenHints == 70)
            {
                return;
            }

            Random rng = new Random();

            int rand;

            do
            {
                rand = rng.Next(81);
            } while (Game.Hints.IndexOf(rand) != -1);

            Game.Hints.Add(rand);

            int ax = rand % 9;
            int ay = (rand - ax) / 9;

            Board[ax][ay].setHint();

            givenHints++;

        }

        static public List<int> Hints = new List<int>(81);

        public static int givenHints = 0;

        public void drawBoard()
        {
            int x = 25;
            int y = 25;

            for (int i = 0; i < 9; i++)
            {
                List<SudokuButton> line = new List<SudokuButton>(9);
                for (int j = 0; j < 9; j++)
                {
                    SudokuButton b = new SudokuButton(Sol.getAnswer(i, j));
                    b.Location = new Point(x, y);
                    b.Size = new Size(70, 70);
                    this.Controls.Add(b);
                    x += 70;
                    if ((j + 1) % 3 == 0 && j > 0)
                    {
                        x += 15;
                    }

                    line.Add(b);
                }
                Board.Add(line);
                x = 25;
                if ((i + 1) % 3 == 0 && i > 0) y += 15;
                y += 70;
            }

            for (int i = 0; i < 17; i++)
            {
                createHint();
            }
            return;
        }

        private void Game_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1000, 750);
            drawBoard();
            CheckerButton winnerButton = new CheckerButton();
            HintButton hinter = new HintButton();
            this.Controls.Add(winnerButton);
            this.Controls.Add(hinter);
        }
    }
}