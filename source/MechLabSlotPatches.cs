﻿using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;
using System.Linq;
using BattleTech.DataObjects;
using HBS.Logging;
using Object = System.Object;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
    public static class MechLabPanelLoadMechPatch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                //GUIModUtils.LogHierarchy(__instance.transform);

                var Representation = __instance.transform.GetChild("Representation");
                var OBJ_mech = Representation.GetChild("OBJ_mech");
                
                var Centerline = OBJ_mech.GetChild("Centerline");
                {
                    var layout_details = Centerline.GetChild("layout_details");
                    if (layout_details == null)
                    {
                        layout_details = Representation.GetChild("layout_details");
                    }
                    if (layout_details != null)
                    {
                        var OBJ_value = Representation.GetChild("OBJ_value");
                        var LeftArmWidget = OBJ_mech.GetChild("LeftArm").GetChild("uixPrfPanl_ML_location-Widget-MANAGED");

                        var v = new Vector3[4];
                        LeftArmWidget.Rect().GetWorldCorners(v);
                        var armtop = v[1].y;
                        var armleft = v[2].x;
                        var armright = v[3].x;
                        var armcenter_x = armleft + (armleft - armright) / 2;

                        layout_details.SetParent(Representation, true);
                        layout_details.Rect().pivot = new Vector2(1.0f, 1.0f);
                        layout_details.position = new Vector3(
                            armcenter_x,
                            armtop + layout_details.Rect().sizeDelta.y + 10,
                            OBJ_value.position.z
                        );
                    }
                }

                const float space = 20;

                {
                    var headWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var centerTorsoWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    centerTorsoWidget.SetTop(headWidget.Bottom() - space);
                }

                {
                    var RightTorsoLeg = OBJ_mech.GetChild("RightTorsoLeg");
                    var RightTorsoWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var RightLegWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    RightLegWidget.SetTop(RightTorsoWidget.Bottom() - space);
                }

                {
                    var LeftTorsoLeg = OBJ_mech.GetChild("LeftTorsoLeg");
                    var LeftTorsoWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var LeftLegWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    LeftLegWidget.SetTop(LeftTorsoWidget.Bottom() - space);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(MechLabLocationWidget), "SetData")]
    public static class MechLabLocationWidgetSetDataPatch
    {
        public static void Postfix(MechLabLocationWidget __instance, int ___maxSlots, LocationLoadoutDef ___loadout)
        {
            try
            {
                // we can't reduce to zero
                if (___maxSlots < 1)
                {
                    return;
                }

                var widget = __instance.transform;

                var layout = widget.GetChild("layout_slots");

                if (layout == null)
                {
                    return;
                }

                var slots = layout.GetChildren()
                    .Where(x => x.name.StartsWith("slot"))
                    .OrderByDescending(x => x.localPosition.y)
                    .ToList();

                var changedSlotCount = ___maxSlots - slots.Count;


                if (changedSlotCount == 0)
                {
                    change_color(slots);
                    return;
                }

                var templateSlot = slots[0];

                // add missing
                for (var i = slots.Count; i < ___maxSlots; i++)
                {
                    var newSlot = UnityEngine.Object.Instantiate(templateSlot, layout);
                    newSlot.localPosition = new Vector3(0, -(1 + i * SlotHeight), 0);
                    newSlot.SetSiblingIndex(templateSlot.GetSiblingIndex());
                    newSlot.name = "slot (" + i + ")";
                }

                // remove abundant
                for (var i = ___maxSlots; i < slots.Count; i++)
                {
                    UnityEngine.Object.Destroy(slots[i].gameObject);
                }

                change_color(slots);


                var changedHeight = changedSlotCount * SlotHeight;

                widget.AdjustHeight(changedHeight);
                layout.AdjustHeight(changedHeight);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        private static void change_color(List<Transform> slots)
        {
            foreach (var transform in slots)
            {
                var color = transform.GetComponentInChildren<BattleTech.UI.UIColorRefTracker>();
                color.SetUIColor(UIColor.Blue);
            }
        }

        private const int SlotHeight = 32;
    }

    public static class Utils
    {
        public static void AdjustHeight(this Transform transform, int changedHeight)
        {
            var rect = transform.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            var vector = rect.sizeDelta;
            vector.y += changedHeight;
            rect.sizeDelta = vector;
        }

        public static RectTransform Rect(this Transform transform)
        {
            return transform.GetComponent<RectTransform>();
        }

        public static float Top(this Transform transform)
        {
            return transform.localPosition.y;
        }

        public static void SetTop(this Transform transform, float y)
        {
            var position = transform.localPosition;
            position.y = y;
            transform.localPosition = position;
        }

        public static float Bottom(this Transform transform)
        {
            var rect = transform.GetComponent<RectTransform>();
            return transform.localPosition.y - rect.sizeDelta.y;
        }

        public static void SetBottom(this Transform transform, float y)
        {
            var rect = transform.GetComponent<RectTransform>();
            var position = transform.localPosition;
            position.y = y + rect.sizeDelta.y;
            transform.localPosition = position;
        }

        public static float Right(this Transform transform)
        {
            var rect = transform.GetComponent<RectTransform>();
            return transform.localPosition.x + rect.sizeDelta.x;
        }

        public static void SetRight(this Transform transform, float x)
        {
            var rect = transform.GetComponent<RectTransform>();
            var position = transform.localPosition;
            position.x = x - rect.sizeDelta.x;
            transform.localPosition = position;
        }

        public static IEnumerable<Transform> GetChildren(this Transform @this)
        {
            foreach (Transform current in @this)
            {
                yield return current;
            }
        }

        public static Transform GetChild(this Transform @this, string name, int index = 0)
        {
            return @this.GetChildren().Where(x => x.name == name).Skip(index).FirstOrDefault();
        }
        
        public static void LogTransform(Transform transform)
        {
            Control.mod.Logger.LogDebug("");
            Control.mod.Logger.LogDebug("name=" + transform.name);
            Control.mod.Logger.LogDebug("parent=" + transform.parent);
            Control.mod.Logger.LogDebug("position=" + transform.position);
            Control.mod.Logger.LogDebug("localPosition=" + transform.localPosition);
            var rect = transform.GetComponent<RectTransform>();
            Control.mod.Logger.LogDebug("rect.anchoredPosition=" + rect.anchoredPosition);
            //Control.mod.Logger.LogDebug("rect.anchorMax=" + rect.anchorMax);
            //Control.mod.Logger.LogDebug("rect.anchorMin=" + rect.anchorMin);
            //Control.mod.Logger.LogDebug("rect.offsetMax=" + rect.offsetMax);
            //Control.mod.Logger.LogDebug("rect.offsetMin=" + rect.offsetMin);
            Control.mod.Logger.LogDebug("rect.pivot=" + rect.pivot);
            Control.mod.Logger.LogDebug("rect.rect=" + rect.rect);
        }

        public static void LogHierarchy(Transform transform, int level = 0)
        {
            var text = "";
            for (var i = 0; i < level; i++)
            {
                text += "  ";
            }
            
            string rectText = "";
            {
                var rect = transform.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rectText = "rect=" + rect.rect + " ancho=" + rect.anchoredPosition;
                }
            }

            Control.mod.Logger.LogDebug(text + transform.gameObject.name + " world=" + transform.position + " local=" + transform.localPosition + " " + rectText);
            level++;
            foreach (Transform current in transform)
            {
                LogHierarchy(current, level);
            }
        }
    }
}