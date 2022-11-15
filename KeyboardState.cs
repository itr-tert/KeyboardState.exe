using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

class EntryPoint {

    static readonly String HelpText = @"
CapsLockなどのキーボードの状態を設定・取得します.

オプション:
  指定した状態に変更します。
  |  [capslock=[on|off]]
  |  [insert=[on|off]]
  |  [numlock=[on|off]]
  |  [scroll=[on|off]]
  [-p | --print-state] 変更前のキーボード状態を表示します.
  [-h | --help] 使い方を表示します.

  引数なしで起動した場合は capslock=off insert=off numlock=on scroll=off を指定した場合と同じです。

  例: KeyboardState.exe -p
  例: KeyboardState.exe capslock=off
".Substring(1);

    [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
    public extern static int MapVirtualKey(int wCode, int wMapType);

    const int MAPVK_VK_TO_VSC = 0;

    [DllImport("user32.dll")]
    public extern static void keybd_event(byte bVk, byte bScan,
					  uint dwFlags, int dwExtraInfo);

    const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    const uint KEYEVENTF_KEYUP = 0x0002;

    const int VK_NUMLOCK = 0x90;
    const int VK_SCROLL = 0x91;
    const int VK_INSERT = 0x2D;
    const int VK_CAPITAL = 0x14;

    static public void keybd_press(byte vk) {
        byte scan = (byte)MapVirtualKey((int)vk, MAPVK_VK_TO_VSC);
	keybd_press(vk, scan);
    }
    static public void keybd_press(byte vk, byte scan) {

	uint extend = 0;
	if ((0x21 <= vk && vk <= 0x2F) ||
	    (0x5B <= vk && vk <= 0x5F) ||
	    (0xA6 <= vk && vk <= 0xB7)) {

	    extend = KEYEVENTF_EXTENDEDKEY;
	}

	keybd_event(vk, scan, extend, 0);
	keybd_event(vk, scan, extend | KEYEVENTF_KEYUP, 1);
    }

    static public void Main(string[] args) {

	Func<String, bool> a = (search) => {
	    for (int len = args.Length, idx = 0;
		 idx < len; ++idx) {
		if (args[idx] == search) {
		    return true;
		}
	    }
	    return false;
	};

	bool show_state = false;
	// 設定するかどうか
	bool set_caps = false;
	bool set_insert = false;
	bool set_num = false;
	bool set_scroll = false;

	// 目的状態
	bool t_caps = false;
	bool t_insert = false;
	bool t_num = false;
	bool t_scroll = false;

	if (a("help") || a("-h") || a("--help") || a("/h") || a("/help") || a("/?")) {

	    Console.Write("{0}", HelpText);
	    return;
	}

	if (args.Length == 0) {
	    show_state = false;
	    set_caps   = true;  t_caps   = false;
	    set_insert = true;  t_insert = false;
	    set_num    = true;  t_num    = true; 
	    set_scroll = true;  t_scroll = false;
	}

	if (a("-p") || a("--print-state")) {
	    show_state = true;
	}
	if (a("capslock=on"))  { set_caps   = true;  t_caps   = true;}
	if (a("capslock=off")) { set_caps   = true;  t_caps   = false;}
	if (a("insert=on"))    { set_insert = true;  t_insert = true;}
	if (a("insert=off"))   { set_insert = true;  t_insert = false;}
	if (a("numlock=on"))   { set_num    = true;  t_num    = true;}
	if (a("numlock=off"))  { set_num    = true;  t_num    = false;}
	if (a("scroll=on"))    { set_scroll = true;  t_scroll = true;}
	if (a("scroll=off"))   { set_scroll = true;  t_scroll = false;}

	// 現在状態
	bool c_caps
	    = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock);
	bool c_insert
	    = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.Insert);
	bool c_num
	    = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.NumLock);
	bool c_scroll
	    = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.Scroll);

	if (show_state) {
	    Console.WriteLine("CapsLock:{0}, InsertMode:{1}, NumLock:{2}, ScrollLock:{3}",
			      c_caps, c_insert, c_num, c_scroll);
	}

	if (set_caps && t_caps != c_caps) {
	    keybd_press(VK_CAPITAL);
	}
	if (set_insert && t_insert != c_insert) {
	    keybd_press(VK_INSERT);
	}
	if (set_num && t_num != c_num) {
	    keybd_press(VK_NUMLOCK);
	}
	if (set_scroll && t_scroll != c_scroll) {
	    keybd_press(VK_SCROLL);
	}
    }
}
