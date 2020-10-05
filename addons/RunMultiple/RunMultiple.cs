
using Godot;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

[Tool]
public class RunMultiple : EditorPlugin
{
	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

	private Button _UiRun;
	private LineEdit _UiLaunchAmount;
	private Button _UiShouldBuild;
	private Button _UiShouldAutoConnect;
	private bool _ShouldBuild = false;
	private bool _ShouldAutoConnect = false;

	public override void _EnterTree()
	{
		GD.Print(nameof(RunMultiple) + " added to project");

		_UiRun = new Button();
		_UiRun.Connect("pressed", this, nameof(_OnButtonRunPressed));
		_UiRun.Text = "Run";
		AddControlToContainer(CustomControlContainer.Toolbar, _UiRun);
		
		_UiLaunchAmount = new LineEdit();
		_UiLaunchAmount.Text = "3";
		AddControlToContainer(CustomControlContainer.Toolbar, _UiLaunchAmount);
		
		_UiShouldBuild = new Button();
		_UiShouldBuild.Connect("pressed", this, nameof(_OnButtonShouldBuildPressed));
		_UiShouldBuild.Text = "Build: No ";
		AddControlToContainer(CustomControlContainer.Toolbar, _UiShouldBuild);
		
		_UiShouldAutoConnect = new Button();
		_UiShouldAutoConnect.Connect("pressed", this, nameof(_OnButtonShouldAutoConnectPressed));
		_UiShouldAutoConnect.Text = "Connect: No ";
		AddControlToContainer(CustomControlContainer.Toolbar, _UiShouldAutoConnect);
	}

	public override void _ExitTree()
	{
		if(_UiRun != null)
	   		 RemoveControlFromContainer(CustomControlContainer.Toolbar, _UiRun);
		if (_UiLaunchAmount != null)
			RemoveControlFromContainer(CustomControlContainer.Toolbar, _UiLaunchAmount);
		if (_UiShouldBuild != null)
			RemoveControlFromContainer(CustomControlContainer.Toolbar, _UiShouldBuild);
		if (_UiShouldAutoConnect != null)
			RemoveControlFromContainer(CustomControlContainer.Toolbar, _UiShouldAutoConnect);
	}

	private void _OnButtonRunPressed()
	{
		int launchAmount = 3;
		int.TryParse(_UiLaunchAmount.Text, out launchAmount);
		_Launch(launchAmount);
	}
	
	private void _OnButtonShouldBuildPressed()
	{
		if (_ShouldBuild)
		{
			_ShouldBuild = false;
			_UiShouldBuild.Text = "Build: No ";
		}
		else
		{
			_ShouldBuild = true;
			_UiShouldBuild.Text = "Build: Yes";
		}
	}
	
	private void _OnButtonShouldAutoConnectPressed()
	{
		if (_ShouldAutoConnect)
		{
			_ShouldAutoConnect = false;
			_UiShouldAutoConnect.Text = "Args: No ";
		}
		else
		{
			_ShouldAutoConnect = true;
			_UiShouldAutoConnect.Text = "Args: Yes";
		}
	}


	private void _Launch(int amount)
	{
		Process process;
		
		if (_ShouldBuild)
		{
			process = _BuildProject();
			process.WaitForExit();	
		}

		if (_ShouldAutoConnect)
		{
			process = _StartProcessGame("-acServer");
			for (int i = 1; i < amount; i++)
			{
				process = _StartProcessGame("-acClient");
			}
		}
		else
		{
			for (int i = 0; i < amount; i++)
			{
				process = _StartProcessGame();
			}
		}
		
	}
	
	private Process _BuildProject()
	{
		string projectPath = System.IO.Path.GetFullPath("./");
		ProcessStartInfo startInfo = new ProcessStartInfo();
		startInfo.FileName = "cmd.exe";
		startInfo.WorkingDirectory = projectPath;
		startInfo.Arguments = "/c \"godot --build-solutions -q\"";
		return Process.Start(startInfo);
	}
	
	private Process _StartProcessGame(string customArgs = "")
	{
		string projectPath = System.IO.Path.GetFullPath("./");
		ProcessStartInfo startInfo = new ProcessStartInfo();
		startInfo.FileName = "cmd.exe";
		startInfo.WorkingDirectory = projectPath;
		startInfo.Arguments = $"/c \"godot -d {customArgs}\"";
		Process newProcess = Process.Start(startInfo);
		return newProcess;
	}
}
