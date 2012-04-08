##############################################################e
####################### How to use ###########################
##
## Place this file in the content\\content folder and exectute
##
##############################################################
##############################################################


import sys
import os as os
root = sys.path[0]
PathFromSource = '''Content/Content'''

def GetFiles(dir_name):
    files = []
    
    for file in os.listdir(dir_name):
        filepath = os.path.join(dir_name, file)

        if os.path.isfile(filepath):
            files.append(filepath)
        elif os.path.isdir(filepath):
            files.extend(GetFiles(filepath))

    return files

def PrintDepth(IndentDepth):
    for i in range(IndentDepth):
        print "   ",

def LegalID(name):
    newname = name.replace(' ', '___').replace('^', '_CARROT_').replace('/', '__IN__').replace("'", '_APOSTROPHE_')
    return newname

Components = []
GUID = 1000
Text = ""
def EncodeDir(dir_name, IndentDepth = 0):
    global GUID
    
    name = os.path.basename(dir_name)
    PrintDepth(IndentDepth)
    Text += '''<Directory Id="%s" Name="%s">\n''' % (LegalID(name), name)

    # Directories first
    for file in os.listdir(dir_name):
        filepath = os.path.join(dir_name, file)
        
        if os.path.isdir(filepath):            
            EncodeDir(filepath, IndentDepth + 1)

    # Then files
    PrintDepth(IndentDepth + 1)
    ComponentName = LegalID('''"%sComponent"\n''' % name)
    Components.append(ComponentName)
    Text += '''<Component Id= %s Guid="48b6ed04-%s-43fa-a4e7-ec7ace50585b" DiskId="1">\n''' % (ComponentName, GUID)
    GUID += 1
    
    for file in os.listdir(dir_name):
        filepath = os.path.join(dir_name, file)
        relativepath = filepath.replace(root, '').replace('\\', '/')
        filename = os.path.basename(filepath)

        if os.path.isfile(filepath):
            PrintDepth(IndentDepth + 2)
            Text += '''<File Id="%sFile" Name="%s" Source="$(sys.SOURCEFILEDIR)/../%s"/>\n''' % (LegalID(relativepath), filename, PathFromSource + relativepath)
    PrintDepth(IndentDepth + 1)
    Text += '''</Component>\n'''

    PrintDepth(IndentDepth)    
    Text += '''</Directory>\n'''

EncodeDir(root)
print Text

print
print
for component in Components:
    print '''<ComponentRef Id=%s />''' % component



