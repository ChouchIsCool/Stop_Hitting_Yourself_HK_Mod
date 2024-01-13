using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using Modding;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Stop_Hitting_Yourself
{
    public class Stop_Hitting_Yourself : Mod
    {
        internal static Stop_Hitting_Yourself Instance;

        //mod variables
        private float swingsBeforeDamage = 6f;

        bool extra = false;
        bool maxHealthFix = false;
        GameObject guy = new GameObject();

        new public string GetName() => "Stop Hitting yourself";

        public override string GetVersion() => "v1.0";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");
            guy.AddComponent<BoxCollider2D>();
            guy.AddComponent<DamageHero>();
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            ModHooks.SoulGainHook += GainSoul;
            ModHooks.AfterAttackHook += AfterAttack;

            Instance = this;
            
            Log("Initialized");
            ModHooks.LanguageGetHook += LanguageGet;

        }


        public int GainSoul(int orig) 
        {

            return (int)((float)orig / 1.25f);
        }

        public void AfterAttack(AttackDirection dir) 
        {
            swingsBeforeDamage--;
        }



        public void OnHeroUpdate()
        {

            if (swingsBeforeDamage <= 0)
            {
                HeroController.instance.TakeDamage(guy, CollisionSide.bottom, 1, 0);
                swingsBeforeDamage = 6f;
                if (extra) swingsBeforeDamage = 8f;
            }

            if (PlayerData.instance.equippedCharm_23 && !maxHealthFix)
            {
                extra = true;
                maxHealthFix = true;
                PlayerData.instance.SetInt("maxHealth", PlayerData.instance.GetInt("maxHealth")-2);
                PlayerData.instance.SetInt("maxHealthBase", PlayerData.instance.GetInt("maxHealthBase") - 2);
                swingsBeforeDamage = 8f;    
            }
            if (!PlayerData.instance.equippedCharm_23 && maxHealthFix)
            {
                extra = false;
                maxHealthFix = false;
                PlayerData.instance.SetInt("maxHealth", PlayerData.instance.GetInt("maxHealth") + 2);
                PlayerData.instance.SetInt("maxHealthBase", PlayerData.instance.GetInt("maxHealthBase") + 2);
                swingsBeforeDamage = 6f;
            }
        }

        public string LanguageGet(string key, string sheet, string orig)
        {

            if (orig.Contains("Fragile Heart"))
            {
                return orig.Replace("Fragile Heart", "Precise Aim");
            }
            if (orig.Contains("Unbreakable Heart"))
            {
                return orig.Replace("Unbreakable Heart", "Unbreakable Precise Aim");
            }
            if (orig.Contains("Increases the health of the bearer, allowing them to take more damage."))
            {
                return orig.Replace("Increases the health of the bearer, allowing them to take more damage.", "Allows the bearer to swing twice more before damaging themself.");
            }

            return orig;
        }

    }
}