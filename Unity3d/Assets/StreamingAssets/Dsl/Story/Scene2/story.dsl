story(story_main)
{
  local
  {
  	@pt(0);
  };
  onmessage("start")
  {
    createnpc(1000,vector3(65,0,70),0,3,1,3,stringlist(""));
    setleaderid(unitid2objid(1000));
    npcsetformation(1000,0);
    npcaddskill(1000,22);
    npcaddskill(1000,23);
    publishgfxevent("ui_add_skill_button", "ui", 1, unitid2objid(1000), 22);
    publishgfxevent("ui_add_skill_button", "ui", 1, unitid2objid(1000), 23);
    publishgfxevent("ui_add_skill_button", "ui", 1, unitid2objid(1000), 0);
    loop(4){
      createnpc(1001+$$,vector3(55+rndint(0,10),0,65+rndint(0,10)),0,3,$$+2,4,stringlist(""),getleaderid());
      npcsetformation(1001+$$,$$+1);
      publishgfxevent("ui_add_skill_button","ui",$$+2,unitid2objid(1001+$$), 0);
    };
    camerafollow(1000);
    wait(1000);    
    showdlg(2);
  };
  onmessage("dialog_over",2)
  {
    log("scene2");
    loop(26){
    	@pt = getcomponent("pt"+($$/7),"PositionMarker").Position;
      createnpc(1006+$$,rndvector3(@pt,10),0,4,$$+6,2,stringlist(""));
    };
    highlightprompt(0,"Tip_1");
  };
  onmessage("move_to")
  {
  	npcmove(1000,vector3($0,$1,$2));
  };
  onmessage("all_killed")
  {
  	firemessage("show_dlg",998);
  	clearmessage("show_dlg");
  };
  onmessage("dialog_over",998)
  {
    terminate();
    changescene(1);
  };
};