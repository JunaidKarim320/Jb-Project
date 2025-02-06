using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationHandler : MonoBehaviour
{
    public CustomizationCategory[] CustomizationCategories;
    private DataController _dataController;


    public void Start()
    {
        _dataController = DataController.instance;
        SetLastSelectedCustomization();
    }

    public void SetLastSelectedCustomization()
    {
        SetLastSelectedGrip(false);
        SetLastSelectedLazer(false);
        SetLastSelectedMuzzle(false);
        SetLastSelectedScope(false);
    }

    public void SetLastSelectedGrip(bool reset)
    {
        int i = 0;
        for (int j = 0; j < CustomizationCategories[i].m_Items.Count; j++)
        {
            if (CustomizationCategories[i].m_Items[j])
                CustomizationCategories[i].m_Items[j].SetActive(false);
        }

        if (!reset)
        {
            if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,
                ApplicationController.SelectedInventoryItem, i, ApplicationController.SelectedGripItem))
            {
                if (CustomizationCategories[i].m_Items[ApplicationController.SelectedGripItem])
                    CustomizationCategories[i].m_Items[ApplicationController.SelectedGripItem].SetActive(true);
            }
        }
    }
    
    public void SetLastSelectedLazer(bool reset)
    {
        int i = 1;
        for (int j = 0; j < CustomizationCategories[i].m_Items.Count; j++)
        {
            if ( CustomizationCategories[i].m_Items[j])
                CustomizationCategories[i].m_Items[j].SetActive(false);
        }

        if (!reset)
        {
            if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,
                ApplicationController.SelectedInventoryItem, i, ApplicationController.SelectedLazerItem))
            {
                if (CustomizationCategories[i].m_Items[ApplicationController.SelectedLazerItem])
                    CustomizationCategories[i].m_Items[ApplicationController.SelectedLazerItem].SetActive(true);
            }
        }
    }
    public void SetLastSelectedMuzzle(bool reset)
    {
        int i = 2;
        for (int j = 0; j < CustomizationCategories[i].m_Items.Count; j++)
        {
            if (CustomizationCategories[i].m_Items[j])
                CustomizationCategories[i].m_Items[j].SetActive(false);
        }

        if (!reset)
        {
            if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,
                ApplicationController.SelectedInventoryItem, i, ApplicationController.SelectedMuzzleItem))
            {
                if (CustomizationCategories[i].m_Items[ApplicationController.SelectedMuzzleItem])
                    CustomizationCategories[i].m_Items[ApplicationController.SelectedMuzzleItem].SetActive(true);
            }
        }
    }
    public void SetLastSelectedScope(bool reset)
    {
        int i = 3;
        for (int j = 0; j < CustomizationCategories[i].m_Items.Count; j++)
        {
            if ( CustomizationCategories[i].m_Items[j])
                CustomizationCategories[i].m_Items[j].SetActive(false);
        }

        if (!reset)
        {
            if (_dataController.GetUnlockInventoryItem(ApplicationController.SelectedInventoryCategory,
                ApplicationController.SelectedInventoryItem, i, ApplicationController.SelectedScopeItem))
            {
                if (CustomizationCategories[i].m_Items[ApplicationController.SelectedScopeItem])
                    CustomizationCategories[i].m_Items[ApplicationController.SelectedScopeItem].SetActive(true);
            }
        }

    }
}
