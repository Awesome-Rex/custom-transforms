﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using REXTools.EditorTools;

namespace REXTools.CustomTransforms
{
    public class CustomTransformHandlesWindow : EditorWindow
    {
        public static System.Type activeType = null;
        public static dynamic activeCustomTransform;


        private bool isVertical
        {
            get
            {
                return position.width < position.height;
            }
        }
        private float maxButtonSize;

        [SerializeField] private Texture positionIcon;
        [SerializeField] private Texture rotationIcon;

        private GUIStyle positionStyle;
        private GUIStyle rotationStyle;

        //methods
        //[MenuItem("Window/Custom Transform Handles")]
        public static void ShowWindow()
        {
            GetWindow<CustomTransformHandlesWindow>("Custom Transform Handles");
        }
        
        private void DrawPositionHandles()
        {
            if (!isVertical) // horizontal
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    float buttonSize = position.height - (EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 3f));
                    float buttonSizeClamped = Mathf.Clamp(buttonSize, 0f, maxButtonSize);

                    GUILayout.Box(GUIContent.none, positionStyle, GUILayout.Height(position.height), GUILayout.Width(position.height));

                    EditorGUILayout.BeginVertical(GUILayout.Width(buttonSizeClamped * 3f));
                    {
                        EditorGUILayout.LabelField("Self", EditorStyles.boldLabel, GUILayout.Width(buttonSizeClamped * 3f));

                        EditorGUILayout.BeginHorizontal();
                        CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), LinkSpaceRotation.Self, ref CustomPositionEditor.selfHandleRot);
                        CustomEditors.EnumButton(this, () => GUILayout.Button("Parent", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), LinkSpaceRotation.Parent, ref CustomPositionEditor.selfHandleRot);
                        CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), LinkSpaceRotation.World, ref CustomPositionEditor.selfHandleRot);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    //vertical line
                    EditorGUILayout.BeginVertical(GUILayout.Width(1f));
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(EditorGUIUtility.singleLineHeight / 2f));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(GUILayout.Width(buttonSizeClamped * 2f));
                    {
                        EditorGUILayout.LabelField("World", EditorStyles.boldLabel, GUILayout.Width(buttonSizeClamped * 2f));
                        
                        EditorGUILayout.BeginHorizontal();
                        CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), Space.Self, ref CustomPositionEditor.worldHandleRot);
                        CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), Space.World, ref CustomPositionEditor.worldHandleRot);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    //vertical line
                    EditorGUILayout.BeginVertical(GUILayout.Width(1f));
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(EditorGUIUtility.singleLineHeight / 2f));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Extra", EditorStyles.boldLabel);

                        CustomPositionEditor.offsetHandleRaw = EditorGUILayout.Toggle("Use Raw Position", CustomPositionEditor.offsetHandleRaw);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            else // vertical
            {
                GUILayout.Box(GUIContent.none, positionStyle, GUILayout.Height(position.width), GUILayout.Width(position.width));

                float buttonSize = position.width - (EditorGUIUtility.standardVerticalSpacing * 2f);

                EditorGUILayout.LabelField("Self", EditorStyles.boldLabel, GUILayout.Width(buttonSize));

                CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), LinkSpaceRotation.Self, ref CustomPositionEditor.selfHandleRot);
                CustomEditors.EnumButton(this, () => GUILayout.Button("Parent", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), LinkSpaceRotation.Parent, ref CustomPositionEditor.selfHandleRot);
                CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), LinkSpaceRotation.World, ref CustomPositionEditor.selfHandleRot);
                
                EditorGUILayout.LabelField("World", EditorStyles.boldLabel, GUILayout.Width(buttonSize));
                CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), Space.Self, ref CustomPositionEditor.worldHandleRot);
                CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), Space.World, ref CustomPositionEditor.worldHandleRot);
                EditorGUILayout.LabelField("Extra", EditorStyles.boldLabel, GUILayout.Width(position.width));

                CustomPositionEditor.offsetHandleRaw = EditorGUILayout.Toggle(new GUIContent(string.Empty, "Use Raw Position"), CustomPositionEditor.offsetHandleRaw, GUILayout.Width(buttonSize));
            }
        }
        private void DrawRotationHandles ()
        {
            if (!isVertical) // horizontal
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    float buttonSize = position.height - (EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 3f));
                    float buttonSizeClamped = Mathf.Clamp(buttonSize, 0f, maxButtonSize);

                    GUILayout.Box(GUIContent.none, rotationStyle, GUILayout.Height(position.height), GUILayout.Width(position.height));

                    EditorGUILayout.BeginVertical(GUILayout.Width(buttonSizeClamped * 3f));
                    {
                        EditorGUILayout.LabelField("Self", EditorStyles.boldLabel, GUILayout.Width(buttonSizeClamped * 3f));

                        EditorGUILayout.BeginHorizontal();
                        CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), LinkSpaceRotation.Self, ref CustomRotationEditor.selfHandleRot);
                        CustomEditors.EnumButton(this, () => GUILayout.Button("Parent", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), LinkSpaceRotation.Parent, ref CustomRotationEditor.selfHandleRot);
                        CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), LinkSpaceRotation.World, ref CustomRotationEditor.selfHandleRot);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    //vertical line
                    EditorGUILayout.BeginVertical(GUILayout.Width(1f));
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(EditorGUIUtility.singleLineHeight / 2f));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(GUILayout.Width(buttonSizeClamped * 2f));
                    {
                        EditorGUILayout.LabelField("World", EditorStyles.boldLabel, GUILayout.Width(buttonSizeClamped * 2f));

                        EditorGUILayout.BeginHorizontal();
                        CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), Space.Self, ref CustomRotationEditor.worldHandleRot);
                        CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSizeClamped), GUILayout.Height(buttonSize)), Space.World, ref CustomRotationEditor.worldHandleRot);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    //vertical line
                    EditorGUILayout.BeginVertical(GUILayout.Width(1f));
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(EditorGUIUtility.singleLineHeight / 2f));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Extra", EditorStyles.boldLabel);

                        CustomRotationEditor.offsetHandleRaw = EditorGUILayout.Toggle("Use Raw Rotation", CustomRotationEditor.offsetHandleRaw);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            else // vertical
            {
                GUILayout.Box(GUIContent.none, rotationStyle, GUILayout.Height(position.width), GUILayout.Width(position.width));

                float buttonSize = position.width - (EditorGUIUtility.standardVerticalSpacing * 2f);

                EditorGUILayout.LabelField("Self", EditorStyles.boldLabel, GUILayout.Width(buttonSize));

                CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), LinkSpaceRotation.Self, ref CustomRotationEditor.selfHandleRot);
                CustomEditors.EnumButton(this, () => GUILayout.Button("Parent", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), LinkSpaceRotation.Parent, ref CustomRotationEditor.selfHandleRot);
                CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), LinkSpaceRotation.World, ref CustomRotationEditor.selfHandleRot);

                EditorGUILayout.LabelField("World", EditorStyles.boldLabel, GUILayout.Width(buttonSize));
                CustomEditors.EnumButton(this, () => GUILayout.Button("Self", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), Space.Self, ref CustomRotationEditor.worldHandleRot);
                CustomEditors.EnumButton(this, () => GUILayout.Button("World", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.MaxHeight(maxButtonSize)), Space.World, ref CustomRotationEditor.worldHandleRot);
                EditorGUILayout.LabelField("Extra", EditorStyles.boldLabel, GUILayout.Width(position.width));

                CustomRotationEditor.offsetHandleRaw = EditorGUILayout.Toggle(new GUIContent(string.Empty, "Use Raw Rotation"), CustomRotationEditor.offsetHandleRaw, GUILayout.Width(buttonSize));
            }
        }

        private void OnEnable()
        {
            rotationStyle = new GUIStyle();
            rotationStyle.normal.background = (Texture2D)rotationIcon;

            positionStyle = new GUIStyle();
            positionStyle.normal.background = (Texture2D)positionIcon;

            maxButtonSize = 60f;
            minSize = new Vector2(maxButtonSize, maxButtonSize + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }

        private Vector2 scrollPos;
        public void OnGUI()
        {
            if (!isVertical) // horizontal
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            }
            else
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUIStyle.none);
            }

            if (activeType == typeof(CustomPosition))
            {
                DrawPositionHandles();
            }
            else if (activeType == typeof(CustomRotation))
            {
                DrawRotationHandles();
            }

            EditorGUILayout.EndScrollView();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}