using UnityEngine;
using System.Collections.Generic;

public class DialogS0
{
    public int id;
    public string charachrtName;
    public string text;
    public int nextId;
    
    public List<DialogChoicesS0> choices = new List<DialogChoicesS0>();
    public Sprite portrait;

    public string portraitPath;
}
