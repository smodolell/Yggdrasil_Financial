using Yggdrasil.Site.Services.Dtos;

namespace Yggdrasil.Site.States;

public class LayoutState
{
    private bool _navIsOpen = true;

    public event Action? NavIsOpenEvent;

    public bool NavIsOpen
    {
        get => _navIsOpen;
        set
        {
            _navIsOpen = value;
            NavIsOpenEvent?.Invoke();
        }
    }


    public event Action<AccessPointDto>? NavToEvent;

    public void NavTo(AccessPointDto item) => NavToEvent?.Invoke(item);
}
