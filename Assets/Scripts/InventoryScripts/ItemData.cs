using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ItemData
{
    public string name;
    public int id;
    public int count;
    [Multiline]
    public string description;         // опис предмету, тобто дл€ чого в≥н в гр≥ використовуЇтьс€ (п≥дказка дл€ гравц€)
    public bool isUniq;

    public ItemData(string name, int id, int count, string description, bool isUniq)
    {
        this.name = name;
        this.id = id;
        this.count = count;
        this.description = description;
        this.isUniq = isUniq;
    }
}
