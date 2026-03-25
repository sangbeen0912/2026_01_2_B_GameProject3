using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="DialogDatabasS0",menuName ="Dialiog System/DialogDatabasS0")]
public class DialogDatabasS0 : ScriptableObject
{
    public List<DialogS0> dialogs = new List<DialogS0>();

    private Dictionary<int,DialogS0> diaiogsByid;

    public void initailize()
    {
        diaiogsByid = new Dictionary<int, DialogS0>();

        foreach(var dialog in dialogs)
        {
            if(dialog != null)
            {
                diaiogsByid[dialog.id] = dialog;
            }
        }
    }

    public DialogS0 GetDiaiogsByid(int id)
    {
        if(diaiogsByid == null)
           initailize();

           if(diaiogsByid.TryGetValue(id, out DialogS0 dialog))
        {
            return dialog;
        }

        return null;
    }
}
