
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;

public class SceneEditor : MonoBehaviour
{
    public Canvas sceneCanvas;
    public RectTransform sceneSelector;
    public GameObject itemSelector;
    public GameObject assetSelector; //stickerSelector
    public Transform startPos;
    public Text placeHolder;

    public GameObject sideBar;
    public GameObject assetDropbar; //stickerSidebar

    public GameObject sceneScroller;
    public GameObject themeScroller;
    public GameObject assets;

    public Button menuBtn;
    public Button bckBtn;
    public Button rmvBtn;

    private Dictionary<string, int> stickerNames = new Dictionary<string, int>();

    bool import = false;
    int counter;

    private Animator anim;
    private Animator animStickers;
    private Dictionary<string, Sprite> stickerSprites = new Dictionary<string, Sprite>();
    private HashSet<string> themeStrings = new HashSet<string>();
    private Dictionary<string, GameObject> themes = new Dictionary<string, GameObject>();
    private GameObject activeTheme;

    private bool dropBarActive = false;
    private bool assetBarActive = false;

    void Start()
    {
        togglePanels(sceneSelector.gameObject, true);

        populateMenus("BackgroundThumbnails", "Scene", sceneScroller);
        addListeners(sceneScroller);
        populateDropbar();
        addListeners(themeScroller);
        

        anim = sideBar.GetComponent<Animator>();
        anim.enabled = false;

        animStickers = assetDropbar.GetComponent<Animator>();
        animStickers.enabled = false;

        //hide buttons until scene background has been selected
        menuBtn.gameObject.SetActive(false);
        rmvBtn.gameObject.SetActive(false);

        //keep back button active 
        bckBtn.gameObject.SetActive(true);

    }

    void addListeners(GameObject section)
    {
        foreach (Transform child in section.transform)
        {
            // Save name of each child to memory
            string captured = child.name;
            string spriteName = child.gameObject.GetComponent<Image>().sprite.name;
            switch (section.name)
            {
                case "BackgroundSelectionPanel":
                    // Add listener to background selections
                    child.GetComponent<Button>().onClick.AddListener(() => changeBackground(captured));
                    break;
                case "ThemePanel":
                    // Add listener to asset categories
                    child.GetComponent<Button>().onClick.AddListener(() => selectAssetCategory(captured));
                    break;
                case "AssetPanel":
                    // Add listeners to resource assets
                    child.GetComponent<Button>().onClick.AddListener(() => addAsset(captured));
                    break;
            }
        }
    }

//This method calls in all assets from the LayoutImages folder in the persistantPath
void addNewAssets(string prefab, GameObject parent)
{
    foreach (string file in System.IO.Directory.GetFiles(Application.persistentDataPath + "/"))
    {
        counter = counter + 1;
        Texture2D tex = null;
        byte[] fileData;
        var fileName = file;
        fileData = File.ReadAllBytes(fileName);
        Debug.Log("File is around:" + file);
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        Debug.Log("tex is: " + tex);

        Rect rec = new Rect(0, 0, tex.width, tex.height);
        GameObject j = Instantiate(Resources.Load(prefab)) as GameObject;
        Sprite temp = Sprite.Create(tex, rec, new Vector2());
        j.GetComponent<Image>().sprite = temp;
        stickerSprites.Add(tex.name + counter, temp);
        stickerSprites[tex.name] = temp;

        j.transform.SetParent(parent.transform);
        j.transform.position = j.transform.parent.position;
        j.name = tex.name;
        Debug.Log(" is: " + tex.name);
        }
    
}
    void changeBackground(string name)
    {
        //add number to the background names
        name = Regex.Replace(name, "[0-9]", "");
        //Append background with scene
        string sceneName = "Backgrounds/" + name + "Scene";
        //create background with instantiated resource
        Texture2D background = Instantiate(Resources.Load(sceneName)) as Texture2D;
        Rect r = new Rect(0, 0, background.width, background.height);
        sceneCanvas.GetComponent<Image>().sprite = Sprite.Create(background, r, new Vector2());
        togglePanels(sceneSelector.gameObject, false);

        menuBtn.gameObject.SetActive(true);
        rmvBtn.gameObject.SetActive(true);
    }

    void selectAssetCategory(string theme)
    {
        // Check if scene resources have already been imported
        if (themeStrings.Contains(theme))
        {
            // If another caegory is active, deactivate it
            if (activeTheme != null && activeTheme != themes[theme])
            {
                //togglePanels(activeTheme, false);
            }

            // Make category visible
            togglePanels(themes[theme], true);
            activeTheme = themes[theme];
            assetSelector = activeTheme;

            // dropbar animation
            animStickers.enabled = true;
            slideIn(animStickers);
            assetBarActive = true;
            return;
        }

        //if the imports theme is selected
        if (theme != "Imports") {
            themeStrings.Add(theme);
            string themeName = "Stickers/" + theme;
            assetBarActive = true;
            deleteChildren(assetSelector);
            populateMenus(themeName, "AssetSelector", assetSelector);
            addListeners(assetSelector);
            animStickers.enabled = true;
            slideIn(animStickers);

            // Instantiate the assets from the category and populate the list
            GameObject go = Instantiate(assetSelector);
            go.name = theme + "Panel";
            //Debug.Log("Theme Add other: " + go);
            themes.Add(theme, go);
            activeTheme = themes[theme];
        }

        if (theme == "Imports") {
            themeStrings.Add(theme);
            string themeName = "Stickers/" + theme;
            assetBarActive = true;
            deleteChildren(assetSelector);
            addNewAssets("AssetSelector", assetSelector);
            addListeners(assetSelector);
            animStickers.enabled = true;
            slideIn(animStickers);

            // Instantiate the stickers from the theme and add them to a list
            GameObject go = Instantiate(assetSelector);
            go.name = theme + "Panel";
            Debug.Log("Theme Add: " + go);
            themes.Add(theme, go);
            activeTheme = themes[theme];
           
        }

    }

    void deleteChildren(GameObject go)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in go.transform)
        {
            children.Add(child.gameObject);
        }
        go.transform.DetachChildren();

        foreach (GameObject g in children)
        {
            stickerSprites.Clear();
            Destroy(g);
        }
    }

    void populateDropbar()
    {
        populateMenus("Themes", "Theme", itemSelector);

        foreach (Transform theme in itemSelector.transform)
        {
            // Remove numbers from file name
            theme.name = Regex.Replace(theme.name, "[0-9]", "");
            Debug.Log(theme.name);
            theme.GetChild(0).GetComponent<Text>().text = theme.name;
        }
    }

    private void slideIn(Animator temp)
    {
        temp.Play("SidePanelDown");
    }

    private void slideOut(Animator temp)
    {
        temp.Play("SidePanelDownOut");
    }

    public void dropBarButton()
    {
        anim.enabled = true;

        if (!assetBarActive)
        {
            if (dropBarActive)
            {
                slideOut(anim);
                dropBarActive = false;
                menuBtn.transform.GetChild(0).GetComponent<Text>().text = "Menu";
            }
            else {
                menuBtn.transform.GetChild(0).GetComponent<Text>().text = "Cancel";
                slideIn(anim);
                dropBarActive = true;
            }
        }
        else {
            slideOut(animStickers);
            assetBarActive = false;
        }
    }

    void addAsset(string name)
    {
        // Create asset and place in scene editor
        GameObject sticker = Instantiate(Resources.Load("Asset")) as GameObject;

        Sprite temp = stickerSprites[name];
        sticker.name = name;
        sticker.GetComponent<Image>().sprite = temp;
        sticker.transform.position = startPos.transform.position;
        sticker.transform.SetParent(assets.transform.parent, true);
    }

    private void populateMenus(string resource, string prefab, GameObject parent)
    {
        
        foreach (Texture2D t in Resources.LoadAll(resource, typeof(Texture2D)))
        {
            Rect r = new Rect(0, 0, t.width, t.height);
            GameObject i = Instantiate(Resources.Load(prefab)) as GameObject;
            Sprite temp = Sprite.Create(t, r, new Vector2());
            i.GetComponent<Image>().sprite = temp;

            if (prefab.Equals("AssetSelector"))
            {
                stickerSprites.Add(t.name, temp);
            }

            i.transform.SetParent(parent.transform);
            i.transform.position = i.transform.parent.position;
            i.name = t.name;
        } 
    }

    public void removePlaceHolder()
    {
        placeHolder.text = "";
    }

    // Deactivate/Reactivate panels
    void togglePanels(GameObject go, bool activate)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        if (activate)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
        else
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }
    }
}
