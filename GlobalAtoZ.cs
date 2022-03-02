using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAtoZ : MonoBehaviour
{
    ///////////////////////////////////////태그 이름/////////////////////////////////////////////
     
    //스테이지 씬
    public static string Tag_DuruContent = "Duru_Content";
    public static string Tag_PlayerOBJ = "Player";
    public static string Tag_player_WATER_Doggabi = "WATER_Doggabi";
    public static string Tag_player_FIRE_Doggabi = "FIRE_Doggabi";
    public static string Tag_player_GRASS_Doggabi = "GRASS_Doggabi";

    //전투 씬
    public static string Tag_Canvas_FightScene = "FightSceneCanvas";
    public static string Tag_Canvas_PlayerMove = "Player_Move_Canvas";
    public static string Tag_FightScene_bg = "FightScene_bg";

    public static string Tag_Attack_FIRE_Doggabi_Fireball0 = "FIRE_Doggabi_Fireball0";
    public static string Tag_Attack_FIRE_Doggabi_Fireball1 = "FIRE_Doggabi_Fireball1";
    public static string Tag_WATER_Doggabi_Shield = "WATER_Doggabi_Shield";
    public static string Tag_Grass_Doggabi_Hill = "GRASS_Doggabi_Hill";
    public static string Tag_Grass_Doggabi_ball0 = "GRASS_Doggabi_ball0";


    public static string Tag_DuOkSiNi_FireSkill1 = "DuOkSiNi_FireSkill1";
    public static string Tag_CameraPosition = "CameraPosition";
    public static string Tag_DUOK_AttackRange = "DUOK_AttackRange";


    ///////////////////////////////////////리소스////////////////////////////////////////////////
    
    //스테이지 씬
    public static string Prefab_Duru = "StageSelectObj/Duru";
    public static string Sprite_opinduru_IMG = "StageSelectObj/openduru_IMG";
    
    //전투 씬
    public static string Sprite_Fight_bg = "FightObj/SceneMap";
    public static string Sprite_Player_img = "FightObj/Player_IMG";
    public static string Sprite_Player_Job_img = "FightObj/Player_job_IMG";
    public static string Sprite_Number_red = "FightObj/damage_num_red";
    public static string Sprite_Number_blue = "FightObj/damage_num_blue";
    public static string Sprite_FireDoggabi_Skill1 = "FightObj/damage_num_blue";

    //스프라이트 애니.
    public static string Anima_Damaged = "FightObj/Damaged_sprite";
    //불도깨비 애니메이션
    public static string Animation_FireDoggabi = "FightObj/Player_Spine/Fire_Doggabi/ReferenceAssets";
    public static string Animation_WaterDoggabi = "FightObj/Player_Spine/Chr_water/ReferenceAssets";
    public static string Animation_GrassDoggabi = "FightObj/Player_Spine/Chr_grass/ReferenceAssets";
    public static string Animation_Duoksini = "FightObj/Player_Spine/Fire_Duoksini/ReferenceAssets";
    public static string Animation_People = "FightObj/Player_Spine/Chr_student/ReferenceAssets";
    public static string Animation_Boar = "FightObj/Enemy_Object/BossBoar_Spine/ReferenceAssets";
    public static string Animation_Wolf = "FightObj/Enemy_Object/Wolf_Spine/ReferenceAssets";

    //적 프리팹
    public static string Prefab_Boar = "FightObj/Enemy_Object/BoarBoss";
    public static string Prefab_Wolf = "FightObj/Enemy_Object/Wolf";

    //공격 도구?
    public static string Prefab_Fireball0 = "FightObj/FireBall0";
    public static string Prefab_Fireball1 = "FightObj/FireBall1";
    public static string Prefab_Watershield = "FightObj/Water_shield";
    public static string Prefab_GrassBall0 = "FightObj/GrassBall0";
    public static string Prefab_Grasshill = "FightObj/Grass_Hill";

    ///////////////////////////////////////////////////////////////////////////////////////////
    public static string String_STAGENUM = "NOW_STAGE";
}
