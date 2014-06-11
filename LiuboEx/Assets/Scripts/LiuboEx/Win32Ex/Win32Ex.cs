#if UNITY_EDITOR || UNITY_STANDALONE_WIN

using UnityEngine;
using System.Collections;

using System;
using System.Runtime.InteropServices;

public class Win32FileDialog
{

    #region Flags
    //static readonly private int OFN_ALLOWMULTISELECT = 0x00000200;
    //static readonly private int OFN_CREATEPROMPT = 0x00002000;
    //static readonly private int OFN_DONTADDTORECENT = 0x02000000;
    //static readonly private int OFN_ENABLEHOOK = 0x00000020;
    //static readonly private int OFN_ENABLEINCLUDENOTIFY = 0x00400000;
    //static readonly private int OFN_ENABLESIZING = 0x00800000;
    //static readonly private int OFN_ENABLETEMPLATE = 0x00000040;
    //static readonly private int OFN_ENABLETEMPLATEHANDLE = 0x00000080;
    //static readonly private int OFN_EXPLORER = 0x00080000;
    //static readonly private int OFN_EXTENSIONDIFFERENT = 0x00000400;
    //static readonly private int OFN_FILEMUSTEXIST = 0x00001000;
    //static readonly private int OFN_FORCESHOWHIDDEN = 0x10000000;
    //static readonly private int OFN_HIDEREADONLY = 0x00000004;
    //static readonly private int OFN_LONGNAMES = 0x00200000;
    //static readonly private int OFN_NOCHANGEDIR = 0x00000008;
    //static readonly private int OFN_NODEREFERENCELINKS = 0x00100000;
    //static readonly private int OFN_NOLONGNAMES = 0x00040000;
    //static readonly private int OFN_NONETWORKBUTTON = 0x00020000;
    //static readonly private int OFN_NOREADONLYRETURN = 0x00008000;
    //static readonly private int OFN_NOTESTFILECREATE = 0x00010000;
    //static readonly private int OFN_NOVALIDATE = 0x00000100;
    //static readonly private int OFN_OVERWRITEPROMPT = 0x00000002;
    //static readonly private int OFN_PATHMUSTEXIST = 0x00000800;
    //static readonly private int OFN_READONLY = 0x00000001;
    //static readonly private int OFN_SHAREAWARE = 0x00004000;
    //static readonly private int OFN_SHOWHELP = 0x00000010;

    private enum FileFlags
    {
        OFN_ALLOWMULTISELECT = 0x00000200,
        OFN_CREATEPROMPT = 0x00002000,
        OFN_DONTADDTORECENT = 0x02000000,
        OFN_ENABLEHOOK = 0x00000020,
        OFN_ENABLEINCLUDENOTIFY = 0x00400000,
        OFN_ENABLESIZING = 0x00800000,
        OFN_ENABLETEMPLATE = 0x00000040,
        OFN_ENABLETEMPLATEHANDLE = 0x00000080,
        OFN_EXPLORER = 0x00080000,
        OFN_EXTENSIONDIFFERENT = 0x00000400,
        OFN_FILEMUSTEXIST = 0x00001000,
        OFN_FORCESHOWHIDDEN = 0x10000000,
        OFN_HIDEREADONLY = 0x00000004,
        OFN_LONGNAMES = 0x00200000,
        OFN_NOCHANGEDIR = 0x00000008,
        OFN_NODEREFERENCELINKS = 0x00100000,
        OFN_NOLONGNAMES = 0x00040000,
        OFN_NONETWORKBUTTON = 0x00020000,
        OFN_NOREADONLYRETURN = 0x00008000,
        OFN_NOTESTFILECREATE = 0x00010000,
        OFN_NOVALIDATE = 0x00000100,
        OFN_OVERWRITEPROMPT = 0x00000002,
        OFN_PATHMUSTEXIST = 0x00000800,
        OFN_READONLY = 0x00000001,
        OFN_SHAREAWARE = 0x00004000,
        OFN_SHOWHELP = 0x00000010,
    }
    #endregion

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;

        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;

        public String file = null;
        public int maxFile = 0;

        public String fileTitle = null;
        public int maxFileTitle = 0;

        public String initialDir = null;

        public String title = null;

        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;

        public String defExt = null;

        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;

        public String templateName = null;

        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    public class FileInfo
    {
        public string MyPath { get; set; }
        public string MyName { get; set; }

        public FileInfo(string _path, string _name)
        {
            MyPath = _path;
            MyName = _name;
        }
    }


    #region 打开文件
    [DllImport("Comdlg32.dll", CharSet = CharSet.Unicode)]

    static private extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    static public FileInfo OpenFileDialog()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        ofn.filter = "All Files\0*.*\0\0";
        //ofn.filter = "图片\0*.jpg;*.png\0";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;

        //ofn.initialDir = UnityEngine.Application.dataPath;//默认路径
        ofn.initialDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        ofn.title = "选择要上传的文件";

        ofn.defExt = "jpg";//显示文件的类型

        //OFN_NOCHANGEDIR项不要缺少
        ofn.flags = (int)FileFlags.OFN_EXPLORER | (int)FileFlags.OFN_FILEMUSTEXIST | (int)FileFlags.OFN_PATHMUSTEXIST | (int)FileFlags.OFN_NOCHANGEDIR;

        if (GetOpenFileName(ofn))
        {
            return new FileInfo(ofn.file, ofn.file.Substring(ofn.fileOffset, ofn.file.Length - ofn.fileOffset));
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region 保存文件
    [DllImport("Comdlg32.dll", CharSet = CharSet.Unicode)]

    static private extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    static public FileInfo SaveFileDialog(string _defalutName = null, string _defalutExt = null)
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        ofn.filter = "All Files\0*.*\0\0";
        //ofn.filter = "图片\0*.jpg;*.png\0";

        if (string.IsNullOrEmpty(_defalutName))
        {
            ofn.file = new string(new char[256]);
        }
        else
        {
            if (_defalutName.Length < 256)
            {
                _defalutName += new string(new char[256 - _defalutName.Length]);
            }
            ofn.file = _defalutName;
        }

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[256]);

        ofn.maxFileTitle = ofn.fileTitle.Length;

        //ofn.initialDir = UnityEngine.Application.dataPath;//默认路径
        ofn.initialDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        ofn.title = "选择要保存的目录";

        ofn.defExt = _defalutExt;//显示文件的类型
        //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        ofn.flags = (int)FileFlags.OFN_OVERWRITEPROMPT | (int)FileFlags.OFN_NOCHANGEDIR;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (GetSaveFileName(ofn))
        {
            return new FileInfo(ofn.file, "");
        }
        else
        {
            return null;
        }
    }
    #endregion
}
#endif