using UnityEngine;
using UnityEngine.Events;

public delegate void DangerEndHandler(IDangerous danger);

public interface IDangerous {

    float DangerRadius { get; }
    Vector3 DangerSource { get; }
    event DangerEndHandler DangerEnd;
}

