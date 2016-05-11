using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using LabyrinthEngine.LevelConstruction;

namespace AndroidGui
{
    [Activity(Label = "AndroidGui", MainLauncher = true, Icon = "@drawable/icon")]
    public class LevelEditorActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Load test playfield
            string boardXmlContent = string.Empty;
            //using (System.IO.Stream input = Assets.Open(@"Assets/levels/BeginnersLament.xml"))
            using (System.IO.Stream input = Assets.Open(@"BeginnersLament2.xml")) // TODO: Use folder instead of root of assets
            {
                using (var reader = new System.IO.StreamReader(input))
                {
                    boardXmlContent = reader.ReadToEnd();
                }
            }

            var boardLoader = new BoardLoader(boardXmlContent);
            var board = boardLoader.Board;

            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.Main);
            SetContentView(new PlayfieldEditorCanvas(this, board));

            // Get our button from the layout resource,
            // and attach an event to it

            //Button button = FindViewById<Button>(Resource.Id.MyButton);
            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }
    }
}

