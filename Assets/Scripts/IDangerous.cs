using UnityEngine;
using UnityEngine.Events;

public class DangerEndEvent : UnityEvent<IDangerous> { }

public interface IDangerous {

    float DangerRadius { get; }
    Vector3 DangerSource { get; }
    DangerEndEvent DangerEnd { get; }

}

