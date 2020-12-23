using UnityEngine;
using UnityEngine.UI;

public class BaseMenuController : MonoBehaviour
{

	protected ServiceManager _serviceManager;

	[SerializeField] protected GameObject _menu;


	[Header("MainButtons")]
	[SerializeField] protected Button _play;
	[SerializeField] protected Button _settings;
	[SerializeField] protected Button _quit;

	[Header("Settings")]
	[SerializeField] protected GameObject _settingsMenu;
	[SerializeField] protected Button _closeSettings;

    // Start is called before the first frame update
    protected virtual void Start()
    {
	    _serviceManager = ServiceManager.Instance;
	    _quit.onClick.AddListener(OnQuitClicked);
	    _settings.onClick.AddListener(OnSettingsClicked);
	    _closeSettings.onClick.AddListener(OnSettingsClicked);
    }

    protected virtual void OnDestroy()
    {
	    _quit.onClick.RemoveListener(OnQuitClicked);
	    _settings.onClick.RemoveListener(OnSettingsClicked);
	    _closeSettings.onClick.RemoveListener(OnSettingsClicked);
    }


    protected virtual void OnMenuClicked()
    {
	    _menu.SetActive(!_menu.activeInHierarchy);
    }

    private void OnQuitClicked()
    {
	    _serviceManager.Quit();
    }

    private void OnSettingsClicked()
    {
	    OnMenuClicked();
	    _settingsMenu.SetActive(!_settingsMenu.activeInHierarchy);
    }
}
