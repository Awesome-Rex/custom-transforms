using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using REXTools.REXCore;
using REXTools.TransformTools;
using REXTools.EditorTools;
using System;

namespace REXTools.CustomTransforms
{
    [UnityEditor.CustomEditor(typeof(CustomPosition))]
    public class CustomPositionEditor : EditorPRO<CustomPosition>
    {
        public static bool offsetHandleRaw = false; //whether or not offset handle position is raw

        public static LinkSpaceRotation selfHandleRot = LinkSpaceRotation.Self; //self rotation
        public static Space worldHandleRot = Space.Self; //world rotation

        //method parameters
        private Space P_Switch_Space;
        private Link P_Switch_Link;

        [SerializeField] private TransformObject P_SwitchParent_Parent;

        private LinkSpace P_SetContext_Type;
        private Vector3 P_SetContext_New;

        private bool P_SwitchFactorScale_Factor;

        private float P_ApplyOffsetScale_NewScale = 1f;

        protected override void DeclareProperties()
        {
            AddProperty("_parent");

            AddProperty("value");

            AddProperty("transition");
            AddProperty("offset");
            AddProperty("link");

            AddProperty("space");

            AddPropertyEditor("P_SwitchParent_Parent");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            target.RecordParent();

            becomeHidden = false;
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() =>
            {
                EditorGUILayout.PropertyField(FindProperty("space"));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
                if (target.space == Space.Self)
                {
                    //target.parent = (Transform)EditorGUILayout.ObjectField("Parent", target.parent, typeof(Transform), true);
                    EditorGUILayout.PropertyField(FindProperty("_parent"));
                }
                if (!(target.space == Space.Self && target.link == Link.Match))
                {

                    //target.value = EditorGUILayout.Vector3Field("Value", target.value);
                    EditorGUILayout.PropertyField(FindProperty("value"));
                }

                EditorGUILayout.Space();

                //Local
                if (target.space == Space.Self)
                {
                    EditorGUILayout.LabelField("Local", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(FindProperty("link"));

                    if (target.link == Link.Offset)
                    {
                        EditorGUILayout.PropertyField(FindProperty("offset"));
                    }

                    target.factorScale = EditorGUILayout.Toggle("Factor Scale?", target.factorScale);

                    DisableGroup(target.factorScale, () =>
                    {
                        target.offsetScale = EditorGUILayout.FloatField("Offset Scale", target.offsetScale);
                    });

                    EditorGUILayout.Space();
                }

                if (target.space == Space.Self && target.link == Link.Offset)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);

                    target.follow = EditorGUILayout.Toggle(string.Empty, target.follow);

                    EditorGUILayout.EndHorizontal();
                    if (target.follow)
                    {
                        EditorGUILayout.PropertyField(FindProperty("transition"));
                    }

                    EditorGUILayout.Space();
                }

                target.showContextInfo = EditorGUILayout.Foldout(target.showContextInfo, "Info", true, EditorStyles.foldout.clone().richText());
                if (target.showContextInfo)
                {
                    GUI.enabled = false;

                    EditorGUILayout.Vector3Field("Position", target.position);
                    if (target.space == Space.Self)
                    {
                        EditorGUILayout.Vector3Field("Local Position", target.localPosition);
                    }

                    if (target.space == Space.Self && target.link == Link.Offset)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.Vector3Field("Position Raw", target.positionRaw);
                        if (target.space == Space.Self)
                        {
                            EditorGUILayout.Vector3Field("Local Position Raw", target.localPositionRaw);
                        }
                    }

                    GUI.enabled = true;

                    EditorGUILayout.Space();
                }

                //Editor methods!

                if (EditorApplication.isPaused || !EditorApplication.isPlaying)
                {
                    //Line();

                    target.showMethods = EditorGUILayout.Foldout(target.showMethods, "Methods", true, EditorStyles.foldout.clone().richText());

                    if (target.showMethods)
                    {
                        if (target.applyInEditor)
                        {
                            GUI.enabled = false;
                        }


                        if (GUILayout.Button("Target to Current"))
                        {
                            Undo.RecordObject(target.gameObject, "Re-Oriented CustomPosition");

                            target.TargetToCurrent();
                        }

                        Function("Switch", () =>
                        {
                            target.Switch(P_Switch_Space, P_Switch_Link);
                        },
                        new Action[] {
                                () => P_Switch_Space = (Space)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Space),
                                () => P_Switch_Link = (Link)EditorGUILayout.EnumPopup(GUIContent.none, P_Switch_Link)
                        }, "Switched CustomPosition Space and/or Link", target.gameObject);

                        Function("Switch Parent", () =>
                        {
                            target.Switch(P_Switch_Space, P_Switch_Link);
                        },
                        new Action[] {
                            () => EditorGUILayout.PropertyField(FindPropertyEditor("P_SwitchParent_Parent"), GUIContent.none)
                        }, "Switched CustomPosition Parent", target.gameObject);

                        Function("Switch Factor Scale", () =>
                        {
                            target.SwitchFactorScale(P_SwitchFactorScale_Factor);
                        },
                        new Action[] {
                                () => P_SwitchFactorScale_Factor = EditorGUILayout.Toggle(GUIContent.none, P_SwitchFactorScale_Factor)
                        }, "Switched CustomPosition FactorScale", target.gameObject);

                        Function("Apply Offset Scale", () =>
                        {
                            target.ApplyOffsetScale(P_ApplyOffsetScale_NewScale);
                        },
                        new Action[] {
                            () => P_ApplyOffsetScale_NewScale = EditorGUILayout.FloatField(GUIContent.none, P_ApplyOffsetScale_NewScale)
                        }, "Applied CustomPosition OffsetScale", target.gameObject);

                        Function("Remove Offset", () =>
                        {
                            if (EditorUtility.DisplayDialog(
                                "Remove Offset?",
                                "Are you sure you want to remove the offset of \"CustomPosition?\"",
                                "Yes", "Cancel"))
                            {
                                Undo.RecordObject(target.gameObject, "Removed CustomPosition Offset");

                                target.RemoveOffset();
                            }
                        },
                        new Action[] { });

                        GUI.enabled = true;

                        if (!target.applyInEditor)
                        {
                            GUI.enabled = false;
                        }

                        Function("Set Property", () =>
                        {
                            if (P_SetContext_Type == LinkSpace.World)
                            {
                                target.position = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.Self)
                            {
                                target.localPosition = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.WorldRaw)
                            {
                                target.positionRaw = P_SetContext_New;
                            }
                            else if (P_SetContext_Type == LinkSpace.SelfRaw)
                            {
                                target.localPositionRaw = P_SetContext_New;
                            }
                        },
                            new Action[] {
                                () => {
                                EditorGUI.BeginChangeCheck();
                                P_SetContext_Type = (LinkSpace)EditorGUILayout.EnumPopup("Type", P_SetContext_Type);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (P_SetContext_Type == LinkSpace.World)
                                    {
                                        P_SetContext_New = target.position;
                                    }
                                    else if (P_SetContext_Type == LinkSpace.Self)
                                    {
                                        P_SetContext_New = target.localPosition;
                                    }
                                    else if (P_SetContext_Type == LinkSpace.WorldRaw)
                                    {
                                        P_SetContext_New = target.positionRaw;
                                    }
                                    else if (P_SetContext_Type == LinkSpace.SelfRaw)
                                    {
                                        P_SetContext_New = target.localPositionRaw;
                                        }
                                    }
                                },
                                () => P_SetContext_New = EditorGUILayout.Vector3Field(GUIContent.none, P_SetContext_New)
                            }, "Changed Context Value of CustomPosition", target.gameObject);

                        GUI.enabled = true;
                    }

                    EditorGUILayout.Space();

                    //Apply button
                    if (!target.applyInEditor)
                    {
                        if (EditorApplication.isPaused)
                        {
                            target.EditorApplyCheck();
                        }

                        if (GUILayout.Button(
                            "Apply in Editor".bold(),
                            EditorStyles.miniButton.clone().richText().fixedHeight(EditorGUIUtility.singleLineHeight * 1.5f)
                            ))
                        {
                            Undo.RecordObject(target.gameObject, "Applied CustomRotation Values in Editor");

                            target.applyInEditor = true;

                            //NEW STUFF
                            //CustomTransformHandlesWindow.ShowWindow();

                            target.EditorApplyCheck();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(
                            "Don't Apply in Editor".colour(Color.red).bold(),
                            EditorStyles.miniButton.clone().richText().fixedHeight(EditorGUIUtility.singleLineHeight * 1.5f)
                            ))
                        {
                            target.applyInEditor = false;

                            target.EditorApplyCheck();
                        }
                    }
                }
            });
        }

        private bool becomeHidden = false;
        private void OnSceneGUI()
        {
            //Debug.Log("SceneGUI");
            //CustomPosition.SetCheckedEnumMenuItems();

            //check if selected is target
            if (
                target.editorApply &&
                Selection.Contains(target.gameObject) &&//Selection.gameObjects.Length == 1 && Selection.activeGameObject == target.gameObject &&
                Tools.current == Tool.Move)
            {
                if (becomeHidden == false)
                {
                    becomeHidden = true;
                }

                if (!Tools.hidden)
                {
                    Tools.hidden = true;
                }
                if (CustomTransformHandlesWindow.activeType != typeof(CustomPosition))
                {
                    CustomTransformHandlesWindow.activeType = typeof(CustomPosition);
                    CustomTransformHandlesWindow.activeCustomTransform = target;
                }
                //if ((CustomPosition)CustomTransformHandlesWindow.activeCustomTransform != target)
                //{
                    
                //}

                Vector3 pos = default;
                Quaternion rot = default;

                if (target.space == Space.World)
                {
                    pos = target.transform.position;
                    if (worldHandleRot == Space.Self)
                    {
                        rot = target.transform.rotation;
                    }
                    else if (worldHandleRot == Space.World)
                    {
                        rot = Quaternion.Euler(Vector3.zero);
                    }
                }
                else
                {
                    if (target.link == Link.Offset)
                    {
                        if (!offsetHandleRaw)
                        {
                            pos = target.position;
                        }
                        else if (offsetHandleRaw)
                        {
                            pos = target.positionRaw;
                        }
                    }
                    else if (target.link == Link.Match)
                    {
                        pos = target.position;
                    }

                    if (selfHandleRot == LinkSpaceRotation.Self)
                    {
                        rot = target.transform.rotation;
                    }
                    else if (selfHandleRot == LinkSpaceRotation.Parent)
                    {
                        rot = target.parent.rotation;
                    }
                    else if (selfHandleRot == LinkSpaceRotation.World)
                    {
                        rot = Quaternion.Euler(Vector3.zero);
                    }
                }

                if (target.link == Link.Offset && offsetHandleRaw)
                {
                    target.positionRaw = Handles.PositionHandle(pos, rot);
                }
                else
                {
                    target.position = Handles.PositionHandle(pos, rot);
                }
            }
            else
            {
                if (becomeHidden == true)
                {
                    Tools.hidden = false;
                    becomeHidden = false;
                    
                    CustomTransformHandlesWindow.activeType = null;
                    CustomTransformHandlesWindow.activeCustomTransform = null;
                }
            }
        }
    }
}