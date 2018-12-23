using System.Diagnostics;

public class ThreadedJob
{
	public string texPath;
	public string imgPath;
	public string workPath;
	public int nextID;

	private bool m_IsDone = false;
	private object m_Handle = new object();
	private System.Threading.Thread m_Thread = null;
	public bool IsDone
	{
		get
		{
			bool tmp;
			lock (m_Handle)
			{
				tmp = m_IsDone;
			}
			return tmp;
		}
		set
		{
			lock (m_Handle)
			{
				m_IsDone = value;
			}
		}
	}
	
	public void Start()
	{
		m_Thread = new System.Threading.Thread(Run);
		m_Thread.Start();
	}

	public void Abort()
	{
		m_Thread.Abort();
	}

	public bool Update()
	{
		if (IsDone)
		{
			return true;
		}
		return false;
	}

	private void Run()
	{
		ThreadFunction();
		IsDone = true;
	}

	private void ThreadFunction()
	{
		string batPath = workPath + "/textopng.bat";
		string errorLogPath = workPath + "/errorLog.txt";
		string outputLogPath = workPath + "/outputLog.txt";
		string args = texPath + " " + nextID + " " + imgPath + " > " + outputLogPath + " 2> " + errorLogPath;

		Process texProc = new Process ();
		var startinfo = new ProcessStartInfo ("cmd.exe","/c" + batPath + " " + args);
		startinfo.CreateNoWindow = true;
		startinfo.UseShellExecute = false;
		startinfo.WorkingDirectory = workPath;
		texProc.StartInfo = startinfo;
		texProc.Start();
		texProc.WaitForExit ();
	}
}
