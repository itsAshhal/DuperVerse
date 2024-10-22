using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{

    public TMP_Text MainCategoryTitle;

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnSetCategory);
    }

    public void OnSetCategory()
    {
        MainCategoryTitle.text = gameObject.name;
    }
}