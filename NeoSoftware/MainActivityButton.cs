using Android.Views;
using Android.Widget;
using Java.Interop;
using Neo.Services;


namespace NeoSoftware
{
    public partial class MainActivity
    {
        private Button _transpose;
        private Button _reverse;
        private Button _det;
        private Button _rank;
        private Button _exponentiation;

        private void GetButtons()
        {
            _transpose = FindViewById<Button>(Resource.Id.transpose_btn);
            _reverse = FindViewById<Button>(Resource.Id.reverse_btn);
            _det = FindViewById<Button>(Resource.Id.det_btn);
            _rank = FindViewById<Button>(Resource.Id.rank_btn);
            _exponentiation = FindViewById<Button>(Resource.Id.exp_btn);
        }

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

        [Export("BackToRecognition")]
        public void BackToRecognitionBtn(View view)
            => SetContentView(Resource.Layout.activity_manual);

        [Export("BackToManual")]
        public void BackToManualBtn(View view) =>
            SetContentView(Resource.Layout.activity_manual);

        [Export("Transpose")]
        public void Transpose(View view)
        {
        }

        [Export("Reverse")]
        public void Reverse(View view)
        {
        }

        [Export("Determinant")]
        public void Determinant(View view)
        {
        }

        [Export("Rank")]
        public void Rank(View view)
        {
        }

        [Export("Exp")]
        public void Exponentiation(View view)
        {
        }
    }
}