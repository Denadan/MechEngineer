using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    public class ChasisInventoryFilter : IFilter<ListElementController_BASE>
    {
        // Token: 0x0600EE1D RID: 60957 RVA: 0x0041F00C File Offset: 0x0041D20C
        public ChasisInventoryFilter(bool allAllowed, bool weaponsAllowed, bool weaponsBallisticAllowed, bool weaponsEnergyAllowed, bool weaponsMissileAllowed, bool weaponsPersonnelAllowed, bool gearAllowed, bool gearHeatSinksAllowed, bool gearJumpJetsAllowed, float mechTonnageForJumpJets, bool gearUpgradesAllowed, bool mechsAllowed)
        {
            this.AllAllowed = allAllowed;
            this.WeaponsAllowed = weaponsAllowed;
            this.WeaponsBallisticAllowed = weaponsBallisticAllowed;
            this.WeaponsEnergyAllowed = weaponsEnergyAllowed;
            this.WeaponsMissileAllowed = weaponsMissileAllowed;
            this.WeaponsPersonnelAllowed = weaponsPersonnelAllowed;
            this.GearAllowed = gearAllowed;
            this.GearHeatSinksAllowed = gearHeatSinksAllowed;
            this.GearJumpJetsAllowed = gearJumpJetsAllowed;
            this.MechTonnageForJumpJets = mechTonnageForJumpJets;
            this.GearUpgradesAllowed = gearUpgradesAllowed;
            this.MechsAllowed = mechsAllowed;
        }

        // Token: 0x0600EE1E RID: 60958 RVA: 0x0041F07C File Offset: 0x0041D27C
        public IEnumerable<ListElementController_BASE> Execute(IEnumerable<ListElementController_BASE> listData)
        {
            foreach (ListElementController_BASE data in listData)
            {
                if (this.AllAllowed)
                {
                    yield return data;
                }
                else
                {
                    bool shouldAllow = false;
                    if (this.WeaponsAllowed)
                    {
                        if (data.weaponDef != null)
                        {
                            WeaponCategory category = data.weaponDef.Category;
                            if (this.WeaponsBallisticAllowed && category == WeaponCategory.Ballistic)
                            {
                                shouldAllow = true;
                            }
                            else if (this.WeaponsEnergyAllowed && category == WeaponCategory.Energy)
                            {
                                shouldAllow = true;
                            }
                            else if (this.WeaponsMissileAllowed && category == WeaponCategory.Missile)
                            {
                                shouldAllow = true;
                            }
                            else if (this.WeaponsPersonnelAllowed && category == WeaponCategory.AntiPersonnel)
                            {
                                shouldAllow = true;
                            }
                        }
                        else if (data.ammoBoxDef != null)
                        {
                            AmmoCategory category2 = data.ammoBoxDef.Ammo.Category;
                            if (this.WeaponsBallisticAllowed && (category2 == AmmoCategory.AC2 || category2 == AmmoCategory.AC5 || category2 == AmmoCategory.AC10 || category2 == AmmoCategory.AC20 || category2 == AmmoCategory.GAUSS))
                            {
                                shouldAllow = true;
                            }
                            else if (this.WeaponsEnergyAllowed && category2 == AmmoCategory.Flamer)
                            {
                                shouldAllow = true;
                            }
                            else if (this.WeaponsMissileAllowed && (category2 == AmmoCategory.LRM || category2 == AmmoCategory.SRM))
                            {
                                shouldAllow = true;
                            }
                            else if (this.WeaponsPersonnelAllowed && (category2 == AmmoCategory.MG || category2 == AmmoCategory.AMS))
                            {
                                shouldAllow = true;
                            }
                        }
                    }
                    if (this.GearAllowed)
                    {
                        if (data.salvageDef != null)
                        {
                            if (this.GearHeatSinksAllowed && data.salvageDef.ComponentType == ComponentType.HeatSink)
                            {
                                shouldAllow = true;
                            }
                            else if (this.GearJumpJetsAllowed && data.salvageDef.ComponentType == ComponentType.JumpJet)
                            {
                                JumpJetDef jumpJetDef = data.salvageDef.MechComponentDef as JumpJetDef;
                                bool flag = this.MechTonnageForJumpJets < 0f || (this.MechTonnageForJumpJets >= jumpJetDef.MinTonnage && this.MechTonnageForJumpJets <= jumpJetDef.MaxTonnage);
                                shouldAllow = flag;
                            }
                            else if (this.GearUpgradesAllowed && data.salvageDef.ComponentType == ComponentType.Upgrade)
                            {
                                shouldAllow = true;
                            }
                        }
                        else if (data.componentDef != null)
                        {
                            if (this.GearHeatSinksAllowed && data.componentDef.ComponentType == ComponentType.HeatSink)
                            {
                                shouldAllow = true;
                            }
                            else if (this.GearJumpJetsAllowed && data.componentDef.ComponentType == ComponentType.JumpJet)
                            {
                                JumpJetDef jumpJetDef2 = data.componentDef as JumpJetDef;
                                bool flag2 = this.MechTonnageForJumpJets < 0f || (this.MechTonnageForJumpJets >= jumpJetDef2.MinTonnage && this.MechTonnageForJumpJets <= jumpJetDef2.MaxTonnage);
                                shouldAllow = flag2;
                            }
                            else if (this.GearUpgradesAllowed && data.componentDef.ComponentType == ComponentType.Upgrade)
                            {
                                shouldAllow = true;
                            }
                        }
                        else if (data.shopDefItem != null)
                        {
                            ComponentType componentType = Shop.ShopItemTypeToComponentType(data.shopDefItem.Type);

                            if (this.GearHeatSinksAllowed && componentType == ComponentType.HeatSink)
                            {
                                shouldAllow = true;
                            }
                            else if (this.GearJumpJetsAllowed && componentType == ComponentType.JumpJet)
                            {
                                JumpJetDef jumpJetDef3 = data.componentDef as JumpJetDef;
                                bool flag3 = this.MechTonnageForJumpJets < 0f || (this.MechTonnageForJumpJets >= jumpJetDef3.MinTonnage && this.MechTonnageForJumpJets <= jumpJetDef3.MaxTonnage);
                                shouldAllow = flag3;
                            }
                            else if (this.GearUpgradesAllowed && componentType == ComponentType.Upgrade)
                            {
                                if (data.componentDef.IsStructure())
                                    shouldAllow = this.MechTonnageForJumpJets < 0f ||
                                                  data.componentDef.GetStructureWeight() == MechTonnageForJumpJets;
                                else
                                    shouldAllow = true;
                            }
                        }
                    }

                    if (this.MechsAllowed)
                    {
                        if (data.salvageDef != null)
                        {
                            if (data.salvageDef.ComponentType == ComponentType.MechPart)
                            {
                                shouldAllow = true;
                            }
                        }
                        else if (data.componentDef != null)
                        {
                            if (data.componentDef.ComponentType == ComponentType.MechPart)
                            {
                                shouldAllow = true;
                            }
                        }
                        else if (data.shopDefItem != null && (data.shopDefItem.Type == ShopItemType.Mech || data.shopDefItem.Type == ShopItemType.MechPart))
                        {
                            shouldAllow = true;
                        }
                    }
                    if (shouldAllow)
                    {
                        yield return data;
                    }
                }
            }
            yield break;
        }

        // Token: 0x04008F21 RID: 36641
        private bool AllAllowed;

        // Token: 0x04008F22 RID: 36642
        private bool WeaponsAllowed;

        // Token: 0x04008F23 RID: 36643
        private bool WeaponsBallisticAllowed;

        // Token: 0x04008F24 RID: 36644
        private bool WeaponsEnergyAllowed;

        // Token: 0x04008F25 RID: 36645
        private bool WeaponsMissileAllowed;

        // Token: 0x04008F26 RID: 36646
        private bool WeaponsPersonnelAllowed;

        // Token: 0x04008F27 RID: 36647
        private bool GearAllowed;

        // Token: 0x04008F28 RID: 36648
        private bool GearHeatSinksAllowed;

        // Token: 0x04008F29 RID: 36649
        private bool GearJumpJetsAllowed;

        // Token: 0x04008F2A RID: 36650
        private float MechTonnageForJumpJets;

        // Token: 0x04008F2B RID: 36651
        private bool GearUpgradesAllowed;

        // Token: 0x04008F2C RID: 36652
        private bool MechsAllowed;


    }
}