using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class RandomizeSprites : MonoBehaviour
{
    [SerializeField] bool sameLegSprites = false;

    private void Start()
    {
        SpriteResolver[] resolverList = GetComponentsInChildren<SpriteResolver>();
        SpriteLibraryAsset libraryAsset = GetComponent<SpriteLibrary>().spriteLibraryAsset;

        bool isLegSelected = false;
        string legLabelName = "";

        for (int i = 0; i < resolverList.Length; i++)
        {
            SpriteResolver resolver = resolverList[i];
            string category = resolver.GetCategory();

            if(sameLegSprites && category == "Leg")
            {
                if (!isLegSelected)
                {
                    isLegSelected = true;
                    string[] labelNames = libraryAsset.GetCategoryLabelNames(category).ToArray();
                    int randomIndex = Random.Range(0, labelNames.Length);
                    legLabelName = labelNames[randomIndex];
                }

                resolver.SetCategoryAndLabel(category, legLabelName);
            }
            else
            {
                string[] labelNames = libraryAsset.GetCategoryLabelNames(category).ToArray();
                int randomIndex = Random.Range(0, labelNames.Length);
                resolver.SetCategoryAndLabel(category, labelNames[randomIndex]);
            }            
        }
    }    
}
