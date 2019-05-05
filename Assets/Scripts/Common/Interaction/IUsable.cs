using UnityEngine;

public interface IUsable {

    GameObject gameObject { get; }

    float range { get ; }

    void Use (Player player);

}
