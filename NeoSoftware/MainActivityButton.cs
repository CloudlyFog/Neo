using Android.Views;
using Android.Widget;
using Java.Interop;
using Neo.Services;

namespace NeoSoftware
{
    public partial class MainActivity
    {
        [Export("Solve")]
        public void OpenConfirmationDialog(View view)
        {
            _detect = false;
            _detectSwitch.Checked = _detect;
            _confirmationAlertDialog.Show();
        }

        [Export("Confirm")]
        public void ConfirmDataInput(View view)
        {
            _detectSwitch.Checked = _detect;
            _confirmationAlertDialog.Cancel();
            _output.Text = new Solver(_tessOutput.Text);
        }

        [Export("Back")]
        public void BackBtn(View view)
        {
            SetContentView(Resource.Layout.activity_manual);
        }
    }
}