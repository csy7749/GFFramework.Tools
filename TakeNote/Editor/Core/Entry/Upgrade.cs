using UnityEngine;

namespace  FuguFirecracker.TakeNote
{
    public static class Upgrade
    {
        public static void FixColors()
        {
            Debug.Log("Updating Ledger");
            foreach (var task in Ledger.Manifest.OutstandingTasks)
                if (task.TextColor == Color.clear)
                    task.TextColor = Color.black;
            
            foreach (var task in Ledger.Manifest.CompletedTasks)
                if (task.TextColor == Color.clear)
                    task.TextColor = Color.black;
            
            foreach (var task in Ledger.Manifest.DeferredTasks)
                if (task.TextColor == Color.clear)
                    task.TextColor = Color.black;
        }
    }
}