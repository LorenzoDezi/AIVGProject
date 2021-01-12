using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAP {

    public static class WorldStateValueExtensions {

        public static List<WorldStateValue> GetUpdatedWith(this List<WorldStateValue> original, List<WorldStateValue> update) {
            List<WorldStateValue> newList = original.ToList();
            newList.UpdateWith(update);
            return newList;
        }

        public static void UpdateWith(this List<WorldStateValue> original, List<WorldStateValue> update) {
            foreach (var state in update) {
                int originalStateIndex = original.IndexOf(state);
                if (originalStateIndex >= 0)
                    original[originalStateIndex] = state;
                else
                    original.Add(state);
            }
        }

    }

}
