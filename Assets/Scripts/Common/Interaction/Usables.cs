using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Usables {
    private ISet<IUsable> usables = new HashSet<IUsable>();

    public IUsable currentUsable {
        get {
            return usables.Where((IUsable usable) => {
                // Only usables in range
                return (usable.gameObject.transform.position - GameManager.instance.player.transform.position).magnitude < usable.range;
            }).OrderBy((IUsable usable) => {
                // Order by distance
                return (usable.gameObject.transform.position - GameManager.instance.player.transform.position).magnitude;
            }).FirstOrDefault(); // Return closest
        }
    }

    public void GrabAllUsables () {
        usables.UnionWith(Object.FindObjectsOfType<MonoBehaviour>().OfType<IUsable>());
    }

    public void ClearUsables () {
        usables.Clear();
    }

    public void AddUsable (IUsable usable) {
        usables.Add(usable);
    }

    public void RemoveUsable (IUsable usable) {
        usables.Remove(usable);
    }
}
