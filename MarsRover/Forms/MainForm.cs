using MarsRover.Services;
using MarsRover.Models;

namespace MarsRover.Forms
{
    public partial class MainForm : Form
    {
        private readonly IRoverService _roverService;
        private List<Command> _commands;
        private Form _form;
        public MainForm(IRoverService roverService)
        {
            _roverService = roverService;
            _commands = new List<Command>();
            _form = new Form();
            StartForm();
        }

        public void StartForm()
        {
            Application.EnableVisualStyles();
             
            _form.Size = new System.Drawing.Size(250, 350);
            _form.BackColor = System.Drawing.Color.DarkGray;

            Rover rover = _roverService.GetRover(0);

            /*Label distanceLabel = CreateLabel(5, 0 ,"Distance");
            TextBox distance = CreateTextBox(distanceLabel.Top, distanceLabel.Right, "distanceText");
            distance.KeyDown += distance_KeyDown;
            Label directionLabel = CreateLabel(distanceLabel.Bottom + 5, 0, "Direction");
            string[] directions = { "North",
                                    "South",
                                    "East",
                                    "West",
                                   };*/



            //ComboBox directionBox = CreateDirectionsList(directionLabel.Top, directionLabel.Right, directions);
            //directionBox.Name = "directionBox";
            Button addButton = CreateButton(5, 60, 200, "Add Command");
            addButton.Click += addButton_Click;
            Label commandCount = CreateLabel(addButton.Bottom + 5, 0, String.Format("There are {0} Commands in the Queue", _commands.Count()), 250);
            commandCount.Name = "commandLabel";

            TextBox commandListBox = CreateTextBox(commandCount.Bottom + 5, 0, "commandListBox");
            commandListBox.ReadOnly = true;
            commandListBox.Multiline = true;
            commandListBox.Height = 85;

            Rover tempRover = _roverService.GetRover(0);

            Label roverLabel = CreateLabel(commandListBox.Bottom + 5, 0, String.Format("The rover is at {0}{1}, {2}", tempRover.Coordinates[0], tempRover.Coordinates[1], tempRover.GetDirection()), 250);
            roverLabel.Name = "roverLabel";

            Button sendButton = CreateButton(5, roverLabel.Bottom + 5, 200, "Send Commands");
            sendButton.Click += sendButton_Click;

            //_form.Controls.Add(distanceLabel);
            //_form.Controls.Add(distance);
            //_form.Controls.Add(directionLabel);
            //_form.Controls.Add(directionBox);

            _form.Controls.Add(commandListBox);
            _form.Controls.Add(addButton);
            _form.Controls.Add(commandCount);
            _form.Controls.Add(roverLabel);
            _form.Controls.Add(sendButton);

            _form.ShowDialog();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(812, 489);
            this.Name = "MainForm";
            this.ResumeLayout(false);
        }

        private Button CreateButton(int left, int top, int width, string input)
        {
            Button button = new Button();
            button.Top = top;
            button.Left = left;
            button.Text = input;
            button.Width = width;
            return button;
        }

        private Label CreateLabel(int top, int right,string input, int width = 0)
        {
            Label label = new Label();
            label.Top = top;
            label.Left = right > 0 ? right - label.Width : 5;
            label.Text = input;
            if(width != 0)
            {
                label.Width = (int)width;
            }
            return label;
        }

        private TextBox CreateTextBox(int top, int right, string name)
        {
            TextBox textBox = new TextBox();
            textBox.Top = top;
            textBox.Left = right + 5;
            textBox.Name = name;
            return textBox;
        }

        private ComboBox CreateDirectionsList(int top, int right, string[] inputs)
        {
            ComboBox directions = new ComboBox();
            directions.Items.AddRange(inputs);
            directions.Top = top;
            directions.Left = right + 5;
            return directions;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            TextBox distance = (TextBox)_form.Controls.Find("distanceText", true).First();
            ComboBox direction = (ComboBox)_form.Controls.Find("directionBox", true).First();
            if (_roverService.CheckDirections(direction.Text, _commands, out string error) == false)
            {
                MessageBox.Show(error);
                return;
            }

            int dis = 0;
            if (_commands.Count() == 5)
            {
                MessageBox.Show("Max commands reached, please send them");
                return;
            }
            if (String.IsNullOrEmpty(distance.Text) == false && String.IsNullOrEmpty(direction.Text) == false)
            {
                AddCommand(int.Parse(distance.Text), direction.Text);
                Label commandCount = (Label)_form.Controls.Find("commandLabel", true).First();
                commandCount.Text = String.Format("There are {0} Commands in the Queue", _commands.Count());

                SetCommandListBox();

                distance.Text = "";
                direction.Text = "";
                return;
            }
            MessageBox.Show("Invalid Input");
            return;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (_roverService.CheckValidMovements(_commands, out string movementError) == false)
            {
                MessageBox.Show(movementError);
                int count = 1;

                SetCommandListBox();

                return;
            }

            Rover tempRover = _roverService.GetRover(0);

            Label roverLabel = (Label)_form.Controls.Find("roverLabel", true).First();

            roverLabel.Text = String.Format("The rover is at {0}{1}, {2}", tempRover.Coordinates[0], tempRover.Coordinates[1], tempRover.GetDirection());

            SetCommandListBox();

            MessageBox.Show("rover moved successfully");

        }

        private void SetCommandListBox()
        {
            TextBox commandListBox = (TextBox)_form.Controls.Find("commandListBox", true).First();

            commandListBox.Text = "";

            int count = 1;
            foreach (Command command in _commands)
            {
                PopulateCommandListBox(command.Distance.ToString(), _roverService.GetStringFromDirection(command.Direction), count, commandListBox);
                count++;
            }
        }

        private void PopulateCommandListBox(string distance, string direction, int count, TextBox commandListBox)
        {
            String contents = commandListBox.Text;

            contents += String.Format("\n{0}: {1} | {2}", count, distance, direction);
            contents += Environment.NewLine;

            commandListBox.Text = contents;
        }

        private void AddCommand(int distance, string direction)
        {
            Command command = new Command(distance, _roverService.GetDirectionFromString(direction));
            _commands.Add(command);
        }
        private void distance_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData != Keys.Oem8)
            {
                if (e.KeyData != Keys.Back)
                {
                    e.SuppressKeyPress = !int.TryParse(Convert.ToString((char)e.KeyData), out int _);
                }
                TextBox distance = (TextBox)sender;

                if(int.TryParse(distance.Text + (char)e.KeyData, out int checkDistance))
                {
                    if (checkDistance < 0)
                    {
                        distance.Text = "0";
                        e.SuppressKeyPress = true;
                    }
                    else if (checkDistance > 99)
                    {
                        distance.Text = "99";
                        e.SuppressKeyPress = true;
                    }
                }

            }
        }
    }
}
