namespace SkillSnap.Client.Services;

public class UserSessionService
{
    public event Action? OnChange;

    private string? _userId;
    public string? UserId
    {
        get => _userId;
        set
        {
            _userId = value;
            NotifyStateChanged();
        }
    }

    private string? _role;
    public string? Role
    {
        get => _role;
        set
        {
            _role = value;
            NotifyStateChanged();
        }
    }

    private int? _currentProjectId;
    public int? CurrentProjectId
    {
        get => _currentProjectId;
        set
        {
            _currentProjectId = value;
            NotifyStateChanged();
        }
    }

    private string? _currentProjectTitle;
    public string? CurrentProjectTitle
    {
        get => _currentProjectTitle;
        set
        {
            _currentProjectTitle = value;
            NotifyStateChanged();
        }
    }

    private bool _isEditingProject;
    public bool IsEditingProject
    {
        get => _isEditingProject;
        set
        {
            _isEditingProject = value;
            NotifyStateChanged();
        }
    }

    public void Clear()
    {
        UserId = null;
        Role = null;
        CurrentProjectId = null;
        CurrentProjectTitle = null;
        IsEditingProject = false;
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}