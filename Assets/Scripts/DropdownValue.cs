using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script permettant de gérer la sélection de difficulté
public class DropdownValue : MonoBehaviour
{
    private Dropdown dropdown;
    private int dropdownIndex;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        //Met à jour la difficulté sélectionnée
        dropdownIndex = dropdown.value;
        Difficulty.selected = Difficulty.difficulties[dropdownIndex];
    }
}
