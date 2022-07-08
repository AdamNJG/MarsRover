using MarsRover.Services;
using MarsRover.Models;
using static MarsRover.Models.Rover;

namespace MarsRover.Forms
{
    public partial class MainForm : Form
    {
        private readonly IRoverService _roverService;
        private List<string> _commands;
        private Form _form;
        public MainForm(IRoverService roverService)
        {
            _roverService = roverService;
            _commands = new List<string>();
            _form = new Form();
            StartForm();
        }

        public void StartForm()
        {
            Application.EnableVisualStyles();
             
            _form.Size = new System.Drawing.Size(250, 350);
            _form.BackColor = System.Drawing.Color.DarkGray;

            Rover rover = _roverService.GetRover();

            Label commandLabel = CreateLabel(5, 0 ,"Command");
            TextBox command = CreateTextBox(commandLabel.Top, commandLabel.Right, "CommandText");


            Button addButton = CreateButton(5, commandLabel.Bottom + 5, 200, "Add Command");
            addButton.Click += addButton_Click;
            Label commandCount = CreateLabel(addButton.Bottom + 5, 0, String.Format("There are {0} Commands in the Queue", _commands.Count()), 250);
            commandCount.Name = "commandLabel";

            TextBox commandListBox = CreateTextBox(commandCount.Bottom + 5, 0, "commandListBox");
            commandListBox.ReadOnly = true;
            commandListBox.Multiline = true;
            commandListBox.Height = 85;

            Label roverLabel = CreateLabel(commandListBox.Bottom + 5, 0, String.Format("The rover is at {0}{1}, {2}", rover.x, rover.y, rover.GetDirection()), 250);
            roverLabel.Name = "roverLabel";

            Button sendButton = CreateButton(5, roverLabel.Bottom + 5, 200, "Send Commands");
            sendButton.Click += sendButton_Click;

            _form.Controls.Add(commandLabel);
            _form.Controls.Add(command);
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

        private void addButton_Click(object sender, EventArgs e)
        {
            TextBox command = (TextBox)_form.Controls.Find("commandText", true).First();

            string commandText = command.Text;
            if (_roverService.ValidateInput(commandText, out string ValidationError))
            {
                _roverService.AddCommand(_commands, commandText, out string addError);
                command.Text = "";
                if (String.IsNullOrEmpty(addError) == false)
                {
                    MessageBox.Show(addError);
                }
                SetCommandListBox();
                return;
            }
            MessageBox.Show(ValidationError);
            return;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            List<string> commandsToRemove = new List<string>();
            foreach (string command in _commands)
            {
                if (_roverService.GetDirectionFromString(command, _roverService.GetTempRover(), out Directions direction))
                {
                    _roverService.GetRover().Direction = direction;
                    commandsToRemove.Add(command);
                }
                else if (_roverService.CheckValidMovements(command, _roverService.GetTempRover(), out string intError))
                {
                    _roverService.SetRoverCoordinates(_roverService.GetRover(), command);
                    commandsToRemove.Add(command);
                }
                else
                {
                    commandsToRemove.Add(command);
                    MessageBox.Show(String.IsNullOrEmpty(intError) ? "Invalid Command, aborting" : intError);
                    break;
                }
            }

            foreach (string usedCommand in commandsToRemove)
            {
                _commands.Remove(usedCommand);
            }

            Label roverLabel = (Label)_form.Controls.Find("roverLabel", true).First();

            Rover rover = _roverService.GetRover();

            roverLabel.Text = String.Format("The rover is at {0}{1}, {2}", rover.y, rover.x, rover.GetDirection());

            SetCommandListBox();

            MessageBox.Show("rover moved successfully");

        }

        private void SetCommandListBox()
        {
            TextBox commandListBox = (TextBox)_form.Controls.Find("commandListBox", true).First();

            commandListBox.Text = "";

            int count = 1;
            foreach (string command in _commands)
            {
                if(int.TryParse(command, out int unusedNumber))
                {
                    PopulateCommandListBox(String.Format("{0}m", command), count, commandListBox);
                    count++;
                }
                else
                {
                    PopulateCommandListBox(command, count, commandListBox);
                    count++;
                }

            }
        }

        private void PopulateCommandListBox(string command, int count, TextBox commandListBox)
        {
            String contents = commandListBox.Text;

            contents += String.Format("\n{0}: {1}", count, command);
            contents += Environment.NewLine;

            commandListBox.Text = contents;
        }
    }
}
