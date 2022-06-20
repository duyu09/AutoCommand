# Source code of AutoCommand_Executer_Core
# Copyright (c) 2022 Qilu University of Technology,Duyu,Chen Yongquan and Liu Jia.杜宇，陈勇全，刘佳，保留所有权利。
# 指令执行模块：分为执行指令集和立即执行单条指令两种模式。另外，本程序还有打包指令集，检查指令集的功能,主程序新建指令集。# 指令集包中所有文件名均必须为英文。
# 指令目录结构：根目录下有文件cmd.xls,setup.config,子目录image。 setup.config第一行为指令时间间隔,第二行为识图可信度,第三行为指令集执行时间(秒),到时间则强制结束,若小于0则不限时。
import os
import sys
import pyautogui
import zipfile
import shutil
import xlrd
import time
import ctypes
import pyperclip
import cv2 # 用于pyautogui精准识图的confidence参数
import threading
from threading import Lock,Thread
from ctypes import cast, POINTER
from comtypes import CLSCTX_ALL
from pycaw.pycaw import AudioUtilities, IAudioEndpointVolume
COPYRIGHT = "Copyright (c) 2022 齐鲁工业大学 计算机科学与技术学院 杜宇，陈勇全"
def UnzipAndCheckCommand(path):
    # 本函数用于解压指令包以及初步检查包，并返回解压后的指令包的或普通指令目录的根目录路径
    if os.path.isfile(path):
        #should be *.acs file.(AutoCommand Command Set File.)
        if not path[path.index("."):] == ".acs":
            print("[ERROR]ILLEGAL_FILE")
            sys.exit(1)
        commandNAME = os.path.basename(path[:path.index(".")])
        temp = zipfile.ZipFile(path,mode='r')
        commandPath = os.getenv("temp") + '\\' + commandNAME
        try:
            shutil.rmtree(commandPath)
        except Exception:
            dfghj = 1
        temp.extractall(commandPath)
        if not os.path.exists(commandPath + "\\cmd.xls"):
            print("[ERROR]ILLEGAL_FILE")
            sys.exit(1)
        return commandPath
    elif os.path.isdir(path):
        if not os.path.exists(path + "\\cmd.xls"):
            print("[ERROR]ILLEGAL_FILE")
            sys.exit(1)
        return path
    else:
        print("[ERROR]ILLEGAL_FILE")
        sys.exit(1)

# xls表格中有3列，分别代表：指令代码，指令参数，重复次数(-1代表死循环)。
# 每行代表一条指令。
# 指令代码：1左键单击 2左键双击 3右键 4输入 5等待 6滚轮 7打开文件 8关闭计算机 9重新启动 10调整操作系统音量 11息屏 12执行用户指定的Windows系统命令行等。
# 重复次数字段仅对1，2，3，5，6，10，11，12有效
# 识图仅对1，2，3，4有效。

# 执行指令集合
def RunSet(commandSetPath,interval_arg,cfd,test):# 指令时间间隔interval_arg单位为秒。test=0为普通执行，test=1为执行临时指令集(ac_temp.xls)
    if cfd <= 0 or cfd > 1:
        print("[ERROR]CFD_ARGS_ERROR")
        sys.exit(1)
    if test == 0:
        path02 = commandSetPath + "\\cmd.xls"
    else:
        path02 = commandSetPath + "\\ac_temp.xls"
    cmdset = xlrd.open_workbook(path02).sheet_by_index(0)
    cmdnum = cmdset.nrows
    # print("-------"+str(cmdnum))
    for i in range(cmdnum):
        cmdtype = int(cmdset.cell(i,0).value)
        cmdtimes = int(cmdset.cell(i,2).value)
        if cmdtimes == 0:
            continue
        if cmdtype == 1:
            # 1=左键单击，指令参数为：字符串"[format]other",format为参数类型，C=坐标值，F=图片文件名；例如[F]01.png [C]12,280 ([C]-1,-1代表当前位置)
            print("[DEBUG]START_EXECUTE_LEFT_CLICK")
            cmdargs = cmdset.cell(i,1).value
            x01,y01 = 0,0
            if cmdargs[1:2] == 'C':
                x01 = int(cmdargs[3:cmdargs.index(",")])
                y01 = int(cmdargs[cmdargs.index(",")+1:])
                if x01 < 0  or y01 < 0:
                    x01,y01 = pyautogui.position()
            elif cmdargs[1:2] == 'F':
                imageName = commandSetPath + r"\image" + '\\' + cmdargs[3:]
                imageName = imageName.replace("\n","")
                if not os.path.exists(imageName):
                    # print(imageName)
                    print("[ERROR]IMAGE_NOT_EXIST")
                    # sys.exit(1)
                # loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
            else:
                print("[ERROR]COMMAND_TYPE_ERROR")
                # sys.exit(1)
            if cmdtimes == -1:
                while True:
                    loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    if loc is None:
                        print("[DEBUG]FINDING_IMAGE")
                        time.sleep(interval_arg)
                        break
                    x01,y01 = loc
                    pyautogui.click(x01,y01,clicks=1,interval=interval_arg,duration=interval_arg,button="left")
            elif cmdtimes >=1:
                for q in range(cmdtimes):
                    loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    while loc is None:
                        print("[DEBUG]FINDING_IMAGE")
                        time.sleep(interval_arg)
                        loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    x01,y01 = loc
                    pyautogui.click(x01,y01,clicks=1,interval=interval_arg,duration=interval_arg,button="left")
            else:
                print("[ERROR]UNKNOWN_ERR")
                # sys.exit(1)

        elif cmdtype == 2:
            # 左键双击，指令参数与左键单击相同。
            print("[DEBUG]START_EXECUTE_LEFT_DOUBLECLICK")
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            x01,y01 = 0,0
            if cmdargs[1:2] == 'C':
                x01 = int(cmdargs[3:cmdargs.index(",")])
                y01 = int(cmdargs[cmdargs.index(",")+1:])
                if x01 < 0 or y01 < 0:
                    x01,y01 = pyautogui.position()
            elif cmdargs[1:2] == 'F':
                imageName = commandSetPath + "\\image\\" + cmdargs[3:]
                imageName = imageName.replace("\n","")
                if not os.path.exists(imageName):
                    print("[ERROR]IMAGE_NOT_EXIST")
                    # sys.exit(1)
                # loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
            else:
                print("[ERROR]COMMAND_TYPE_ERROR")
                # sys.exit(1)
            if cmdtimes == -1:
                while True:
                    loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    if loc is None:
                        print("[DEBUG]FINDING_IMAGE")
                        time.sleep(interval_arg)
                        break
                    x01,y01 = loc
                    pyautogui.click(x01,y01,clicks=2,interval=interval_arg,duration=interval_arg,button="left")
            elif cmdtimes >=1:
                for q in range(cmdtimes):
                    loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    while loc is None:
                        print("[DEBUG]FINDING_IMAGE")
                        time.sleep(interval_arg)
                        loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    x01,y01 = loc
                    pyautogui.click(x01,y01,clicks=2,interval=interval_arg,duration=interval_arg,button="left")
            else:
                print("[ERROR]UNKNOWN_ERR")
                # sys.exit(1)

        elif cmdtype == 3:
            #右键单击，指令参数与左键单击相同。
            print("[DEBUG]START_EXECUTE_RIGHT_CLICK")
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            x01,y01 = 0,0
            if cmdargs[1:2] == 'C':
                x01 = int(cmdargs[3:cmdargs.index(",")])
                y01 = int(cmdargs[cmdargs.index(",")+1:])
                if x01 < 0 or y01 < 0:
                    x01,y01 = pyautogui.position()
            elif cmdargs[1:2] == 'F':
                imageName = commandSetPath + "\\image\\" + cmdargs[3:]
                if not os.path.exists(imageName):
                    print("[ERROR]IMAGE_NOT_EXIST")
                    # sys.exit(1)
                # loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
            else:
                print("[ERROR]COMMAND_TYPE_ERROR")
                # sys.exit(1)
            if cmdtimes == -1:
                while True:
                    loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    if loc is None:
                        print("[DEBUG]FINDING_IMAGE")
                        time.sleep(interval_arg)
                        break
                    x01,y01 = loc
                    pyautogui.click(x01,y01,clicks=1,interval=interval_arg,duration=interval_arg,button="right")
            elif cmdtimes >=1:
                for q in range(cmdtimes):
                    loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    while loc is None:
                        print("[DEBUG]FINDING_IMAGE")
                        time.sleep(interval_arg)
                        loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                    x01,y01 = loc
                    pyautogui.click(x01,y01,clicks=1,interval=interval_arg,duration=interval_arg,button="right")
            else:
                print("[ERROR]UNKNOWN_ERR")
                # sys.exit(1)
                
        elif cmdtype == 4:
            # 4=输入，指令参数为：[format]args[I]sentences。其中[format]args与1相同,注意，若format=N,则在当前焦点处输入,sentences为输入的内容。例如[F]04.jpg[I]自动执行助手软件;[C]-1,-1[I]为便捷办公服务;[N][I]软件开发小组。
            print("[DEBUG]START_EXECUTE_INPUT")
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            context = cmdargs[cmdargs.index("[I]")+3:]
            x01,y01 = 0,0
            if cmdargs[1:2] == 'N':
                pyperclip.copy(context)
                pyautogui.hotkey('ctrl','v') # 通过ctrl+v实现从剪贴板里粘贴出数据。
                pyautogui.hotkey('enter')
                continue
            elif cmdargs[1:2] == 'C':
                x01 = int(cmdargs[3:cmdargs.index(",")])
                y01 = int(cmdargs[cmdargs.index(",")+1:cmdargs.index("[I]")])
                if x01 < 0 or y01 < 0:
                    x01,y01 = pyautogui.position()
                pyperclip.copy(context)
                pyautogui.click(x01,y01,clicks=1,button="left") # 转移焦点到此处
                pyautogui.hotkey('ctrl','v') # 通过ctrl+v实现从剪贴板里粘贴出数据。
                pyautogui.hotkey('enter')
                continue
            elif cmdargs[1:2] == 'F':
                imageName = commandSetPath + "\\image\\" + cmdargs[3:cmdargs.index("[I]")]
                if not os.path.exists(imageName):
                    print("[ERROR]IMAGE_NOT_EXIST")
                    # sys.exit(1)
                loc = pyautogui.locateCenterOnScreen(imageName,confidence=cfd)
                if not loc is None: # 如果没有找到图片位置，则不再继续执行。
                    x01,y01 = loc
                    pyperclip.copy(context)
                    pyautogui.click(x01,y01,clicks=1,button="left") # 转移焦点到此处
                    pyautogui.hotkey('ctrl','v') # 通过ctrl+v实现从剪贴板里粘贴出数据。
                    pyautogui.hotkey('enter')
                    continue
            else:
                print("[ERROR]UNKNOWN_ERR")
                # sys.exit(1)

        elif cmdtype == 5:
            # 5=等待，参数：等待时长(秒)，注意：该指令可重复。
            print("[DEBUG]START_EXECUTE_WAITING")
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            waitInterval = float(cmdargs)
            if cmdtimes == -1:
                while True:
                    time.sleep(interval_arg)
            elif cmdtimes >= 1:
                for w in range(cmdtimes):
                    time.sleep(waitInterval) # 此时不使用interval_args参数。
            else:
                print("[ERROR]UNKNOWN_ERR")
                # sys.exit(1)

        elif cmdtype == 6:
            # 6=滚轮滚动，指令参数：像素数,持续时间(秒)。例如：50,4
            print("[DEBUG]START_EXECUTE_SCROLL")
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            pix = int(cmdargs[:cmdargs.index(",")])
            dur = float(cmdargs[cmdargs.index(",")+1:])
            if cmdtimes == -1:
                while True:
                    pyautogui.scroll(pix,duration = dur)
                    time.sleep(interval_arg)
            elif cmdtimes >= 1:
                for w in range(cmdtimes):
                    pyautogui.scroll(pix,duration = dur)
                    time.sleep(interval_arg)
            else:
                print("[ERROR]UNKNOWN_ERR")
                # sys.exit(1)

        elif cmdtype == 7:
            # 7=打开文件，参数：文件路径。(阻塞式的打开)
            cmdargs = cmdset.cell(i,1).value
            if cmdtimes == -1:
                while True:
                    os.system(cmdargs)
                    # 不使用时间间隔参数
            elif cmdtimes >= 1:
                for w in range(cmdtimes):
                    os.system(cmdargs)
            else:
                print("[ERROR]UNKNOWN_ERR")
                # sys.exit(1)
        elif cmdtype == 8:
            # 8=关机：分为正常关机和暴力关机。参数：[Q]暴力关机，[P]正常关机(可缺省) 重复次数参数无效。
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            if cmdargs == "[Q]":
                dll = ctypes.windll.LoadLibrary("ntdll")
                a001 = ctypes.c_int(19)
                b001 = ctypes.c_int(1)
                c001 = ctypes.c_int(0)
                d001 = ctypes.c_int(0)
                dll.RtlAdjustPrivilege(ctypes.byref(a001),ctypes.byref(b001),ctypes.byref(c001),ctypes.byref(d001))
                # 调用RtlAdjustPrivilege接口函数提升强制关机权限
                dll.NtShutdownSystem(0)
                # 强制关机
                dll.NtShutdownSystem(0)
                os.system("shutdown -f -s -t 0")
                # 若调用NtShutdownSystem失败，则再使用普通强制关机
                os.system("shutdown -f -s -t 0")
                sys.exit(0)
            else:
                os.system("shutdown -s -t 0")
                os.system("shutdown -s -t 0")
                sys.exit(0)
        elif cmdtype == 9:
            # 9=重新启动，暂不支持暴力重启，故没有任何参数。
            os.system("shutdown -r -t 0")
            os.system("shutdown -r -t 0")
            sys.exit(0)
        elif cmdtype == 10:
            # 10=调整Windows系统音量,指令参数为音量百分比，若小于零则默认为零，若大于100则为100.不支持重复次数。
            print("[DEBUG]START_EXECUTE_SETVOL")
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            val_vol = int(cmdargs)
            if val_vol>100:
                val_vol = 100
            if val_vol<0:
                val_vol = 0
            # 字典dic存储了百分比和分贝值的对应关系。
            dic = {0: -65.25, 1: -56.99, 2: -51.67, 3: -47.74, 4: -44.62, 5: -42.03, 6: -39.82, 7: -37.89, 8: -36.17, 9: -34.63, 10: -33.24,
 11: -31.96, 12: -30.78, 13: -29.68, 14: -28.66, 15: -27.7, 16: -26.8, 17: -25.95, 18: -25.15, 19: -24.38, 20: -23.65,
 21: -22.96, 22: -22.3, 23: -21.66, 24: -21.05, 25: -20.46, 26: -19.9, 27: -19.35, 28: -18.82, 29: -18.32, 30: -17.82,
 31: -17.35, 32: -16.88, 33: -16.44, 34: -16.0, 35: -15.58, 36: -15.16, 37: -14.76, 38: -14.37, 39: -13.99, 40: -13.62,
 41: -13.26, 42: -12.9, 43: -12.56, 44: -12.22, 45: -11.89, 46: -11.56, 47: -11.24, 48: -10.93, 49: -10.63, 50: -10.33,
 51: -10.04, 52: -9.75, 53: -9.47, 54: -9.19, 55: -8.92, 56: -8.65, 57: -8.39, 58: -8.13, 59: -7.88, 60: -7.63,
 61: -7.38, 62: -7.14, 63: -6.9, 64: -6.67, 65: -6.44, 66: -6.21, 67: -5.99, 68: -5.76, 69: -5.55, 70: -5.33,
 71: -5.12, 72: -4.91, 73: -4.71, 74: -4.5, 75: -4.3, 76: -4.11, 77: -3.91, 78: -3.72, 79: -3.53, 80: -3.34,
 81: -3.15, 82: -2.97, 83: -2.79, 84: -2.61, 85: -2.43, 86: -2.26, 87: -2.09, 88: -1.91, 89: -1.75, 90: -1.58,
 91: -1.41, 92: -1.25, 93: -1.09, 94: -0.93, 95: -0.77, 96: -0.61, 97: -0.46, 98: -0.3, 99: -0.15, 100: 0.0}
            devices = AudioUtilities.GetSpeakers()
            interface = devices.Activate(IAudioEndpointVolume._iid_, CLSCTX_ALL, None)
            volume = cast(interface, POINTER(IAudioEndpointVolume))
            volume.SetMute(0, None)
            val_absvol = int(dic[val_vol])
            volume.SetMasterVolumeLevel(val_absvol, None)

        elif cmdtype == 11:
            # 11=息屏，无任何参数
            print("[DEBUG]START_EXECUTE_SHUTSCREEN")
            HWND_BROADCAST = 0xffff
            WM_SYSCOMMAND = 0x0112
            SC_MONITORPOWER = 0xF170
            MonitorPowerOff = 2
            SW_SHOW = 5
            ctypes.windll.user32.PostMessageW(HWND_BROADCAST, WM_SYSCOMMAND,SC_MONITORPOWER, MonitorPowerOff)
            shell32 = ctypes.windll.LoadLibrary("shell32.dll")
            shell32.ShellExecuteW(None, 'open', 'rundll32.exe','USER32', '', SW_SHOW)

        elif cmdtype == 12:
            # 12=执行Windows命令行，指令参数即为命令行，可阻塞重复执行。
            if cmdtimes == -1:
                while True:
                    os.system(cmdargs)
                    time.sleep(interval_arg)
            else:
                for sdfg in range(cmdtimes):
                    os.system(cmdargs)
                    time.sleep(interval_arg)
    os.system("taskkill /F /PID " + str(os.getpid()) + " >nul")
    sys.exit(0)


def CheckSet(path):
    # 可初步检查指令集
    abspath01 = UnzipAndCheckCommand(path)
    print(abspath01) # 主进程接收此标准输出，获取路径。
    if not (os.path.exists(abspath01 + r"\cmd.xls") and os.path.exists(abspath01 + r"\setup.config") and os.path.exists(abspath01 + r"\image")):
        print("[CHECK]incomplete")
        sys.exit(1)
    config = open(abspath01 + r"\setup.config")
    i = float(config.readline())
    p = float(config.readline())
    if i<0:
        print("[CHECK]Interval")
        sys.exit(1)
    if p<0 or p>1:
        print("[CHECK]CFD")
        sys.exit(1)
    config.close()
    # 主要是查图片1，2，3，4。
    cmdset = xlrd.open_workbook(abspath01 + r"\cmd.xls").sheet_by_index(0)
    cmdnum = cmdset.nrows
    for i in range(cmdnum):
        cmdtype = int(cmdset.cell(i,0).value)
        if cmdtype == 1 or cmdtype == 2 or cmdtype == 3:
            cmdargs = cmdset.cell(i,1).value
            cmdargs = cmdargs.replace("\n","")
            if cmdargs[1:2] == 'F':
                imageName = path + "\\image\\" + cmdargs[3:]
                if not os.path.exists(imageName):
                    print("[CHECK]IMAGE_NOT_EXIST")
            sys.exit(1)
        if cmdtype == 4:
            if cmdargs[1:2] == 'F':
                # print("---------"+cmdargs)
                imageName = path + "\\image\\" + cmdargs[3:cmdargs.index("[I]")]
                if not os.path.exists(imageName):
                    print("[ERROR]IMAGE_NOT_EXIST")
            sys.exit(1)


def off(second):
    if second > 0:
        end = time.time() + second
        while True:
            time.sleep(0.05)
            if time.time() > end:
                break
        # sys.exit(0)
        os.system("taskkill /F /PID " + str(os.getpid()) + " >nul")
        os.kill()
        sys.exit(0)

#打包指令集
def zip_file(src_dir, save_name='default'):
    if save_name == 'default':
        zip_name = src_dir + '.acs'
    else:
        if save_name is None or save_name == '':
            zip_name = src_dir + '.acs'
        else:
            zip_name = save_name + '.acs'

    z = zipfile.ZipFile(zip_name, 'w', zipfile.ZIP_DEFLATED)
    for dirpath, dirnames, filenames in os.walk(src_dir):
        fpath = dirpath.replace(src_dir, '')
        fpath = fpath and fpath + os.sep or ''
        for filename in filenames:
            z.write(os.path.join(dirpath, filename), fpath + filename)
    z.close()


if __name__ == '__main__':
    # 主函数
    # 命令行参数：1.指令集路径 2.运行模式：0=正常执行，1=执行单独一条(需要有ac_temp.xls) 2=打包指令集 3=检查指令集
    arg01 = sys.argv[1]
    arg02 = sys.argv[2]
    path = UnzipAndCheckCommand(arg01)
    if int(arg02) == 0:
        config = open(path + r"\setup.config")
        i = float(config.readline())
        p = float(config.readline())
        t = float(config.readline())
        shuter = threading.Thread(target=off, args=(t,))
        shuter.start()
        RunSet(path,i,p,0)
        os.system("taskkill /F /PID " + str(os.getpid()) + " >nul")
    elif int(arg02) == 1:
        config = open(path + r"\setup.config")
        i = float(config.readline())
        p = float(config.readline())
        RunSet(path,i,p,1) #一定要在指令集目录下有ac_temp.xls
    elif int(arg02) == 2:
        zip_file(path)
    elif int(arg02) == 3:
        CheckSet(path) # 主程序读取其标准输出。

# End of Source code file of AutoCommand_Executer_Core
                    


            
