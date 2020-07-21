﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using REXTools.CustomTransforms;
using REXTools.REXCore;
using REXTools.TransformTools;

namespace REXTools.CustomTransforms
{
    public class CustomRotation : CustomTransformLinks<Quaternion>
    {
        public Quaternion rotation
        {
            get
            {
                return GetRotation(Space.World);
            }

            set
            {
                if (!(space == Space.Self && link == Link.Offset))
                {
                    if (space == Space.World)
                    {
                        this.value = value;
                    }
                    operationalRotation = SetRotation(value.eulerAngles, Space.World);
                }
                else
                {
                    this.value = SetRotationLocal(offset.ReverseRotation(value).eulerAngles, Space.World);
                }
            }
        }
        public Quaternion localRotation
        {
            get
            {
                return GetRotation(Space.Self);
            }
            set
            {
                if (space == Space.Self)
                {
                    if (link != Link.Offset)
                    {
                        operationalRotation = SetRotation(value.eulerAngles, Space.Self);
                    }
                    else
                    {
                        this.value = SetRotationLocal(offset.ReverseRotation(SetRotation(value.eulerAngles, Space.Self)).eulerAngles, Space.World);
                    }
                }
                else
                {
                    rotation = value;
                }
            }
        }

        public Vector3 eulerAngles
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = Quaternion.Euler(value);
            }
        }
        public Vector3 localEulerAngles
        {
            get
            {
                return localRotation.eulerAngles;
            }
            set
            {
                localRotation = Quaternion.Euler(value);
            }
        }

        public Quaternion rotationRaw
        {
            get
            {
                if (space == Space.Self && link == Link.Offset)
                {
                    return GetRotationRaw(Space.World);
                }
                else
                {
                    return rotation;
                }
            }

            set
            {
                if (space == Space.Self && link == Link.Offset)
                {
                    this.value = SetRotationRawLocal(value.eulerAngles, Space.World);
                }
                else
                {
                    rotation = value;
                }
            }
        }
        public Quaternion localRotationRaw
        {
            get
            {
                if (space == Space.Self && link == Link.Offset)
                {
                    return GetRotationRaw(Space.Self);
                }
                else
                {
                    return localRotation;
                }
            }
            set
            {
                if (space == Space.Self && link == Link.Offset)
                {
                    this.value = SetRotationRawLocal(value.eulerAngles, Space.Self);
                }
                else
                {
                    localRotation = value;
                }
            }
        }

        public Vector3 eulerAnglesRaw
        {
            get
            {
                return rotationRaw.eulerAngles;
            }
            set
            {
                rotationRaw = Quaternion.Euler(value);
            }
        }
        public Vector3 localEulerAnglesRaw
        {
            get
            {
                return localRotationRaw.eulerAngles;
            }
            set
            {
                localRotationRaw = Quaternion.Euler(value);
            }
        }

        private Quaternion operationalRotation
        {
            get
            {
                return transform.rotation;
            }
            set
            {
                transform.rotation = value;
            }
        }

        public Vector3 up
        {
            get
            {
                return (rotation * Vector3.up).normalized;
            }
            set
            {
                rotation = (Quaternion.LookRotation(value) * Quaternion.Euler(90f, 0f, 0f));
            }
        }
        public Vector3 forward
        {
            get
            {
                return (rotation * Vector3.forward).normalized;
            }
            set
            {
                rotation = Quaternion.LookRotation(value);
            }
        }
        public Vector3 right
        {
            get
            {
                return (rotation * Vector3.right).normalized;
            }
            set
            {
                rotation = (Quaternion.LookRotation(value) * Quaternion.Euler(0f, -90f, 0f));
            }
        }

        private Quaternion parentRot;

        public override void SetToTarget()
        {
            //Debug.Log("Rotation - " + value.eulerAngles);

            target = GetTarget();

            if (enabled)
            {
                operationalRotation = target;

                RecordParent();
            }
        }
        public override void MoveToTarget()
        {
            target = GetTarget();

            if (enabled)
            {
                if (space == Space.World)
                {
                    operationalRotation = target;
                }
                else if (space == Space.Self)
                {
                    if (link == Link.Offset)
                    {
                        if (!follow)
                        {
                            operationalRotation = target;
                        }
                        else
                        {
                            operationalRotation = transition.MoveTowards(operationalRotation, target);
                        }
                    }
                    else if (link == Link.Match)
                    {
                        if (REXCore._ETERNAL.I.counter)
                        {
                            operationalRotation = target;
                        }
                    }
                }

                if (_ETERNAL.I.counter)
                {
                    RecordParent();
                }
            }
        }
        public override Quaternion GetTarget()
        {
            Quaternion target = new Quaternion();

            if (space == Space.World)
            {
                target = value;
            }
            else if (space == Space.Self)
            {
                if (link == Link.Offset)
                {
                    target = parentRot * value; //++++++++offset
                    target = offset.ApplyRotation(this, target);
                }
                else if (link == Link.Match)
                {
                    //if (!editorApply) {
                    SetPrevious();
                    //}

                    //target = parentRot * previous; //WORKS!
                    target = Linking.TransformEuler(previous, parent.rotation);
                }
            }

            return target;
        }

        public override void TargetToCurrent()
        {
            if (space == Space.Self)
            {
                if (link == Link.Offset)
                {
                    rotation = operationalRotation;
                }
                else if (link == Link.Match)
                {
                    //already set!!!
                    //++++++++++++++++++++++MAKE A DEBUG.LOG or EXCEPTION
                }
            }
            else if (space == Space.World)
            {
                value = operationalRotation;
            }
        }

        public override void RecordParent()
        {
            parentRot = parent.rotation;
        }

        public Quaternion Rotate(Vector3 eulers, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                return operationalRotation * Quaternion.Euler(eulers); //WORKS!
            }
            else
            {
                return Quaternion.Euler(eulers) * operationalRotation; //WORKS!
            }
        }
        public Quaternion Rotate(Quaternion from, Vector3 eulers, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                return from * Quaternion.Euler(eulers); //WORKS!
            }
            else
            {
                return Quaternion.Euler(eulers) * from; //WORKS!
            }
        }

        public Quaternion SetRotation(Vector3 rotation, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                return parentRot * Quaternion.Euler(rotation); //WORKS!
            }
            else
            {
                return Quaternion.Euler(rotation); //WORKS!
            }
        }
        public Quaternion SetRotationLocal(Vector3 rotation, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                return Quaternion.Euler(rotation);
            }
            else
            {
                return Linking.InverseTransformEuler(Quaternion.Euler(rotation), parentRot);
            }
        }
        public Quaternion GetRotation(Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                return Quaternion.Inverse(parentRot) * operationalRotation; //WORKS!
            }
            else
            {
                return operationalRotation; //WORKS!
            }
        }

        public Quaternion SetRotationRaw(Vector3 rotation, Space relativeTo = Space.Self)
        {
            return SetRotation(rotation, relativeTo);
        }
        public Quaternion SetRotationRawLocal(Vector3 rotation, Space relativeTo = Space.Self)
        {
            return SetRotationLocal(SetRotation(SetRotationLocal(rotation, relativeTo).eulerAngles, Space.Self).eulerAngles, Space.World);
        }
        public Quaternion GetRotationRaw(Space relativeTo = Space.Self)
        {
            if (space == Space.Self && link == Link.Offset)
            {
                if (relativeTo == Space.Self)
                {
                    return SetRotation(offset.ReverseRotation(this, /*SetRotation(GetRotation(relativeTo).eulerAngles, relativeTo)*/ target).eulerAngles, Space.Self);
                }
                else
                {
                    return offset.ReverseRotation(this, target);
                }
            }
            else
            {
                if (space == Space.Self)
                {
                    //return GetRotation(relativeTo);
                    return SetRotationLocal(target.eulerAngles, Space.World);
                }
                else // relative to world
                {
                    return SetRotation(target.eulerAngles, Space.World);
                }
            }
        }

        public override void SetPrevious()
        {
            previous = Linking.InverseTransformEuler(operationalRotation, parentRot);
        }

        public override void Switch(Space newSpace, Link newLink)
        {
            Quaternion originalRotation = rotation;
            Quaternion originalLocalRotation = localRotation;

            if (space == Space.World)
            {
                if (newSpace == Space.Self)
                {
                    if (newLink == Link.Offset) //world > offset
                    {
                        space = Space.Self;
                        link = Link.Offset;

                        //auto keep offset
                        SetToTarget();
                        value = offset.ReverseRotation(this, Linking.InverseTransformEuler(originalRotation, parent.rotation));
                    }
                    else if (newLink == Link.Match) //world > match
                    {
                        space = Space.Self;
                        link = Link.Match;
                    }
                }
            }
            else if (space == Space.Self)
            {
                if (link == Link.Offset)
                {
                    if (newSpace == Space.World) //offset > world
                    {
                        space = Space.World;
                        rotation = originalRotation;
                    }
                    else
                    {
                        if (newLink == Link.Match) //offset > match
                        {
                            link = Link.Match;
                        }
                    }
                }
                else if (link == Link.Match)
                {
                    if (newSpace == Space.World) //match > world
                    {
                        space = Space.World;
                        rotation = originalRotation;
                    }
                    else
                    {
                        if (newLink == Link.Offset) //match > offset
                        {
                            link = Link.Offset;

                            //auto keep offset
                            SetToTarget();

                            value = offset.ReverseRotation(this, Linking.InverseTransformEuler(originalRotation, parent.rotation));
                        }
                    }
                }
            }
        }
        public override void SwitchParent(Transform newParent)
        {
            if (space == Space.Self)
            {
                Quaternion originalRotation = rotation;
                Quaternion originalLocalRotation = localRotation;

                if (link == Link.Offset)
                {
                    parent = newParent;

                    rotation = offset.ReverseRotation(originalRotation);

                }
                else if (link == Link.Match)
                {
                    parent = newParent;

                    rotation = originalRotation;
                }
            }
        }
        public override void RemoveOffset()
        {
            if (space == Space.Self && link == Link.Offset)
            {
                rotation = offset.ApplyRotation(this, rotation);
            }

            offset = new AxisOrder(null, offset.variety, offset.space);
        }

        private void Start() { }
    }
}