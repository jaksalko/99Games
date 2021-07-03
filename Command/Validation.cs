using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Validation
{
    private int _step; public int Step
    {
        get => _step;
        set => _step = value;
    }
    
    private PlayerProperty _propertyA; public PlayerProperty PropertyA
    {
        get => _propertyA;
        set => _propertyA = value;
    }
    private PlayerProperty _propertyB; public PlayerProperty PropertyB
    {
        get => _propertyB;
        set => _propertyB = value;
    }
    private List<List<int>> _datas;

    public List<List<int>> Datas
    {
        get => _datas;
        set => _datas = value;
    }
    private List<List<bool>> _check; public List<List<bool>> Check
    {
        get => _check;
        set => _check = value;
    }
    public Validation(int step,PlayerProperty ppA, PlayerProperty ppB,List<List<int>> datas,List<List<bool>> check)
    {
        _datas = datas.ToList();
        _propertyA = ppA;
        _propertyB = ppB;
        _check = check.ToList();
        _step = step;
    }

    
    
}
