using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


public class TextureGrabberWindow : EditorWindow
{
    [MenuItem("Tools/Texture Grabber Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<TextureGrabberWindow>();
        window.titleContent = new GUIContent("Texture Grabber");
        window.minSize = new Vector2(1100, 800);
    }

    private void OnEnable()
    {
        var original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/WC_TextureSortingTool/Editor/TextureGrabber.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/WC_TextureSortingTool/Editor/TextureGrabberStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        CreateTextureListView();
    }

    private void CreateTextureListView()
    {
        FindAllTextures(out List<Texture> textures);
        
        var textureList = rootVisualElement.Query<ListView>("texture-list").First();
        
        // supply our list view with the textures we found. provide bindings that will show the textures name in list view
        textureList.makeItem = () => new Label();
        textureList.bindItem = (element, i) => (element as Label).text = textures[i].name;

        // TODO finish hooking up the inBuildToggle
        var inBuildToggle = rootVisualElement.Query<Toggle>("InBuild-Toggle").First();
        inBuildToggle.label = "Only Get Textures Used In Build";

        textureList.itemsSource = textures;
        textureList.fixedItemHeight = 16f;
        
        // define the lists selection type
        textureList.selectionType = SelectionType.Single;

        // when selecting an element in the list we call this function
        textureList.onSelectionChange += (enumerable) =>
        {
            // select asset in project view
            Selection.activeObject = (Object)textureList.selectedItem;
            
            foreach (var it in enumerable)
            {
                // clear the textureInfoBox
                var textureInfoBox = rootVisualElement.Query<Box>("texture-info").First();
                textureInfoBox.Clear();

                var texture = it as Texture;
                
                var serializedTexture = new SerializedObject(texture);
                var textureProperty = serializedTexture.GetIterator();
                textureProperty.Next(true);

                while (textureProperty.NextVisible(false))
                {
                    var prop = new PropertyField(textureProperty);
                    
                    prop.SetEnabled(textureProperty.name != "m_Script");
                    prop.Bind(serializedTexture);
                    textureInfoBox.Add(prop);

                    // check if the serialized property name is texture image so we can read the texture and display it
                    if (textureProperty.name == "texture-image")
                    {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) =>
                            LoadTextureImage(texture));
                    }
                }

                LoadTextureImage(texture);
            }
        };
    }

    private void FindAllTextures(out List<Texture> textures)
    {
        // we search the asset database for all assets of type:texture
        var guids = AssetDatabase.FindAssets("t:texture");

        textures = new List<Texture>();

        // fill list with each texture
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            textures.Add(AssetDatabase.LoadAssetAtPath<Texture>(path));
        }
        
        // TODO filter the textures by usage (in build or not)

        // sort the textures by size
        for (int i = 0; i < textures.Count - 1; i++)
        {
            for (int j = 0; j < textures.Count - i - 1; j++)
            {
                if (EditorTextureUtil.GetStorageMemorySize(textures[j]) <
                    EditorTextureUtil.GetStorageMemorySize(textures[j + 1]))
                {
                    // swap textures position in list
                    (textures[j], textures[j + 1]) = (textures[j + 1], textures[j]);
                }
            }
        }
    }

    private void LoadTextureImage(Texture texture)
    {
        var texturePreviewImage = rootVisualElement.Query<Image>("preview").First();
        texturePreviewImage.image = texture;
    }
}
