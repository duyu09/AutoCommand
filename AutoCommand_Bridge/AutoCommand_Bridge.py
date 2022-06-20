import xlrd
import xlwt
import os
import sys
def read(filepath):# 输出为临时目录下的AC_BRI_OUT.tmp
    cmdset = xlrd.open_workbook(filepath).sheet_by_index(0)
    cmdnum = cmdset.nrows
    try:
        os.kill(os.getenv("temp") + r"\AC_BRI_OUT.tmp")
    except:
        duyu = 666
    fo = open(os.getenv("temp") + r"\AC_BRI_OUT.tmp",mode='w')
    if cmdnum ==0:
        fo.close()
        sys.exit(0)
    for i in range(cmdnum):
        cmdtype = '%02d' % int(cmdset.cell(i,0).value)
        cmdtimes = '%03d' % int(cmdset.cell(i,2).value)
        cmdargs = cmdset.cell(i,1).value
        fo.write(cmdtype + " " + cmdtimes + " " +str(cmdargs) +"\n")
    fo.close()
def write(filepath):# 输入文件为临时目录下的AC_BRI_IN.tmp
    fi = open(os.getenv("temp") + r"\AC_BRI_IN.tmp",mode = 'r',encoding='utf-8-sig',errors='ignore')
    allcom = fi.readlines()
    fi.close()
    workbook = xlwt.Workbook(encoding="utf-8")
    cmdset = workbook.add_sheet('0')
    for i in range(len(allcom)):
        strq = allcom[i]
        cmdtype = int(strq[:2])
        cmdtimes = int(strq[3:6])
        cmdargs = strq[7:]
        cmdset.write(i,0,cmdtype)
        cmdset.write(i,1,cmdargs)
        cmdset.write(i,2,cmdtimes)
    try:
        os.kill(filepath)
    except:
        duyu = 666
    workbook.save(filepath)

if __name__ == '__main__':
    # 主函数
    # 命令行参数，1.filepath 2.执行模式：0=读表格，写临时文件；1=读临时文件，写表格。
    if int(sys.argv[2]) == 0:
        read(sys.argv[1])
    if int(sys.argv[2]) == 1:
        write(sys.argv[1])
    sys.exit(0)
    
