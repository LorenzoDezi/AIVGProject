using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponAmmoCounter : MonoBehaviour
{
    [SerializeField]
    private WeaponController weaponController;
    private TMP_Text text;

    private void Awake() {
        text = GetComponent<TMP_Text>();
        weaponController.AmmoChanged += OnAmmoChanged;
    }

    private void OnAmmoChanged(int currAmmo) {
        text.text = string.Concat(" ", currAmmo);
    }

}
