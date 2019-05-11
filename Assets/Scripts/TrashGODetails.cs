using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrashGOMode
{
    NORMAL, SELECTED, FADING, REAPPEARING, ALMOST_GONE, INACTIVE
}

public class TrashMaterials {
    public Material[] normalMaterials;
    public Material[] selectedMaterials;
    public Material[] fadingMaterials;
    public Material[] almostGoneMaterials;
}
public class TrashGODetails
{
    private Material[] goNormalMats;
    private Material[] goSelectedMats;
    private Material[] goFadingMats;
    private Material[] goAlmostGoneMats;

    private GameObject gObject;
    private Renderer goRender;
    private TrashGOMode goMode = TrashGOMode.NORMAL;
    private TrashGOMode previousGOMode = TrashGOMode.NORMAL;
    private Vector3 maxGOScale = new Vector3();
    private bool isHit = false;
    public TrashGOMode GOMode
    {
        get
        {
            return goMode;
        }

        set
        {
            goMode = value;
        }
    }

    public Renderer GORender
    {
        get
        {
            return goRender;
        }

        set
        {
            goRender = value;
        }
    }

    public bool IsHit
    {
        get
        {
            return isHit;
        }

        set
        {
            isHit = value;
        }
    }

    public GameObject GObject
    {
        get
        {
            return gObject;
        }

        set
        {
            gObject = value;
        }
    }

    public TrashGOMode PreviousGOMode
    {
        get
        {
            return previousGOMode;
        }

        set
        {
            previousGOMode = value;
        }
    }

    public Vector3 MaxGOScale
    {
        get
        {
            return maxGOScale;
        }

        set
        {
            maxGOScale = value;
        }
    }

    public Material[] GONormalMats
    {
        get
        {
            return goNormalMats;
        }

        set
        {
            goNormalMats = value;
        }
    }

    public Material[] GOSelectedMats
    {
        get
        {
            return goSelectedMats;
        }

        set
        {
            goSelectedMats = value;
        }
    }

    public Material[] GOFadingMats
    {
        get
        {
            return goFadingMats;
        }

        set
        {
            goFadingMats = value;
        }
    }

    public Material[] GOAlmostGoneMats
    {
        get
        {
            return goAlmostGoneMats;
        }

        set
        {
            goAlmostGoneMats = value;
        }
    }
}

