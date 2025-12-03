using Microsoft.Extensions.DependencyInjection;

namespace MindBodyDictionaryMobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());
}
