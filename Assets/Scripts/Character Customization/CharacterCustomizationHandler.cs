using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationHandler : MonoBehaviour
{

    #region Instance

    private static CharacterCustomizationHandler _instance;

    public static CharacterCustomizationHandler instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterCustomizationHandler>();
            }

            return _instance;
        }
    }

    #endregion
    
     public List<CharacterCustomizationCategory> CustomizationCategories;
    private DataController _dataController;


    public void Start()
    {
        _dataController = DataController.instance;
        
        SetLastSelectedCustomization();
    }
    public void UnlockAllCustomizationItem()
    {
        unlockCustomizationCategoryItem(CharacterCustomizationController.CharacterCustomizationPart.Body);
        unlockCustomizationCategoryItem(CharacterCustomizationController.CharacterCustomizationPart.Face);
        unlockCustomizationCategoryItem(CharacterCustomizationController.CharacterCustomizationPart.Head);
        unlockCustomizationCategoryItem(CharacterCustomizationController.CharacterCustomizationPart.Torso);
    }

    public void unlockCustomizationCategoryItem(CharacterCustomizationController.CharacterCustomizationPart categories)
    {
        for (int k = 0; k < CustomizationCategories[(int)categories].m_CustomizationCategory.Count; k++)
        {
            for (int z = 0; z < CustomizationCategories[(int) categories].m_CustomizationCategory[k].m_CustomizationItemInfo.Count; z++)
            {
                DataController.instance.SetUnlockInventoryItem((int) categories, k, z);
            }
        }
    }
    
    
    
    public void SetLastSelectedCustomization()
    {
        SetLastSelectedHead(false);
        SetLastSelectedGlasses(false);
        SetLastSelectedMostaches(false);
        SetLastSelectedBeard(false);
        SetLastSelectedBody(false);
        SetLastSelectedTorso(false);
    }

    public void SetLastSelectedHead(bool reset)
    {
        int Category = 0;
        for (int j = 0; j < CustomizationCategories[Category].m_CustomizationCategory.Count; j++)
            {
                for (int z = 0; z < CustomizationCategories[Category].m_CustomizationCategory[j].m_Items.Count; z++)
                {
                    if(CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[z])
                    CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[z].SetActive(false);
                }

                if (!reset)
                {
                    if (_dataController.GetUnlockInventoryItem(Category, j, ApplicationController.SelectedHeadItem))
                    {
                        if(CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[ApplicationController.SelectedHeadItem])
                        CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[ApplicationController.SelectedHeadItem].SetActive(true);
                    }
                }
            }
    }
    
    public void SetLastSelectedGlasses(bool reset)
    {
        int Category = 1;
        int subCategory = 0;
        for (int z = 0; z < CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items.Count; z++)
            {
                if( CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[z])
                CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[z].SetActive(false);
            }

        if (!reset)
        {
            if (_dataController.GetUnlockInventoryItem(Category, subCategory, ApplicationController.SelectedGlassesItem))
            {
                if( CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[ApplicationController.SelectedGlassesItem])
                CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[ApplicationController.SelectedGlassesItem].SetActive(true);
            }
        }

    }
    public void SetLastSelectedMostaches(bool reset)
    {
        int Category = 1;
        int subCategory = 1;
        for (int z = 0; z < CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items.Count; z++)
        {
            if( CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[z])
            CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[z].SetActive(false);
        }

        if (!reset)
        {
            if (_dataController.GetUnlockInventoryItem(Category, subCategory, ApplicationController.SelectedMostacheItem))
            {
                if(CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[ApplicationController.SelectedMostacheItem])
                CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[ApplicationController.SelectedMostacheItem].SetActive(true);
            }
        }
    }
    public void SetLastSelectedBeard(bool reset)
    {
        int Category = 1;
        int subCategory = 2;
        for (int z = 0; z < CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items.Count; z++)
        {
            if( CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[z])
            CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[z].SetActive(false);
        }

        if (!reset)
        {
            if (_dataController.GetUnlockInventoryItem(Category, subCategory, ApplicationController.SelectedBeardItem))
            {
                if( CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[ApplicationController.SelectedBeardItem])
                CustomizationCategories[Category].m_CustomizationCategory[subCategory].m_Items[ApplicationController.SelectedBeardItem].SetActive(true);
            }
        }

    }
    public void SetLastSelectedBody(bool reset)
    {
        int Category = 2;
        for (int j = 0; j < CustomizationCategories[Category].m_CustomizationCategory.Count; j++)
        {
            for (int z = 0; z < CustomizationCategories[Category].m_CustomizationCategory[j].m_Items.Count; z++)
            {
                if( CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[z])
                CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[z].SetActive(false);
            }

            if (!reset)
            {
                if (_dataController.GetUnlockInventoryItem(Category, j, ApplicationController.SelectedBodyItem))
                {
                    if(CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[ApplicationController.SelectedBodyItem])
                    CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[ApplicationController.SelectedBodyItem].SetActive(true);
                }
            }
        }
            
    }    
    public void SetLastSelectedTorso(bool reset)
    {
        int Category = 3;
        for (int j = 0; j < CustomizationCategories[Category].m_CustomizationCategory.Count; j++)
        {
            for (int z = 0; z < CustomizationCategories[Category].m_CustomizationCategory[j].m_Items.Count; z++)
            {
                if(CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[z])
                CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[z].SetActive(false);
            }

            if (!reset)
            {
                if (_dataController.GetUnlockInventoryItem(Category, j, ApplicationController.SelectedTorsoItem))
                {
                    if(CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[ApplicationController.SelectedTorsoItem])
                    CustomizationCategories[Category].m_CustomizationCategory[j].m_Items[ApplicationController.SelectedTorsoItem].SetActive(true);
                }
            }
        }
    }
    
    
}