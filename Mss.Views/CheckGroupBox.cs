using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mss.Views
{
    public class CheckGroupBox : GroupBox
    {
        private readonly CheckBox _checkBox;

        public CheckGroupBox()
        {
            // Remove the normal GroupBox text; we'll use the checkbox text instead
//             base.Text = string.Empty;

            _checkBox = new CheckBox
            {
                AutoSize = true,
                Location = new Point(8, -1), // sits in the "header" area
                Checked = true
            };
            _checkBox.CheckedChanged += _CheckBox_CheckedChanged;

            // Add the checkbox to the GroupBox
            Controls.Add(_checkBox);

            // Make sure children reflect initial state
            _UpdateChildControlEnabledState();
        }

        // Expose the checkbox text as the GroupBox Text property
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Appearance")]
        public override string Text
        {
            get => _checkBox.Text;
            set => _checkBox.Text = value;
        }

        // Expose a Checked property
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool Checked
        {
            get => _checkBox.Checked;
            set => _checkBox.Checked = value;
        }

        // Optional: event when Checked changes
        [Category("Behavior")]
        public event EventHandler CheckedChanged;

        private void _CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _UpdateChildControlEnabledState();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        private void _UpdateChildControlEnabledState()
        {
            foreach (Control ctl in Controls)
            {
                if (!ReferenceEquals(ctl, _checkBox))
                {
                    ctl.Enabled = _checkBox.Checked;
                }
            }
        }

        // Make sure new controls added at design/runtime respect the "checked" state
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (!ReferenceEquals(e.Control, _checkBox))
            {
                e.Control.Enabled = _checkBox.Checked;

                // Optionally push everything down a bit so they don't overlap the checkbox
                if (e.Control.Top < _checkBox.Bottom + 4)
                {
                    e.Control.Top = _checkBox.Bottom + 4;
                }
            }
        }

        // Optional: keep checkbox positioned nicely if the control is resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Pin checkbox near left/top of border
            _checkBox.Location = new Point(8, -1);
        }
    }
}
