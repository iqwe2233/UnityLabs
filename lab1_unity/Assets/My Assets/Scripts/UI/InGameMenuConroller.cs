using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuConroller : BaseMenuController
{
	
	[SerializeField] private Button _restart;
	[SerializeField] private Button _backToMenu;



    // Start is called before the first frame update
    protected override void Start()
    {
	    base.Start();
	    _play.onClick.AddListener(OnMenuClicked);
	    _restart.onClick.AddListener(OnRestartClicked);
	    _backToMenu.onClick.AddListener(OnMainMenuClicked);
    }

    protected override void OnDestroy()
    {
	    _restart.onClick.RemoveListener(OnRestartClicked);
	    _backToMenu.onClick.RemoveListener(OnMainMenuClicked);
	    _play.onClick.RemoveListener(OnMenuClicked);
    }

    // Update is called once per frame
    protected  void Update()
    {
	    if (Input.GetKeyUp(KeyCode.Escape))
		    OnMenuClicked();
    }

    protected override void OnMenuClicked()
    {
	    base.OnMenuClicked();
	    Time.timeScale = _menu.activeInHierarchy ? 0 : 1;
    }

    public void OnMainMenuClicked()
    {
	    ServiceManager.Instance.ChangeLvl((int)Scenes.MainMenu);
    }

    private void OnRestartClicked()
    {
	    _serviceManager.Restart();
    }


}
