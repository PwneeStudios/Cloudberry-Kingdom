##############################################################
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
def EncodeDir(dir_name, IndentDepth = 0):
    global GUID
    
    name = os.path.basename(dir_name)
    PrintDepth(IndentDepth)
    print '''<Directory Id="''' + LegalID(name) + '''" Name="''' + name + '''">'''

    # Directories first
    for file in os.listdir(dir_name):
        filepath = os.path.join(dir_name, file)
        
        if os.path.isdir(filepath):            
            EncodeDir(filepath, IndentDepth + 1)

    # Then files
    PrintDepth(IndentDepth + 1)
    ComponentName = LegalID('''"''' + name + '''Component"''')
    Components.append(ComponentName)
    print '''<Component Id=''' + ComponentName + ''' Guid="48b6ed04-''' + str(GUID) + '''-43fa-a4e7-ec7ace50585b" DiskId="1">'''
    GUID += 1

    for file in os.listdir(dir_name):
        filepath = os.path.join(dir_name, file)
        relativepath = filepath.replace(root, '').replace('\\', '/')
        filename = os.path.basename(filepath)

        if os.path.isfile(filepath):
            PrintDepth(IndentDepth + 2)
            print '''<File Id="''' + LegalID(relativepath) + '''File" Name="''' + filename + '''" Source="$(sys.SOURCEFILEDIR)/../''' + PathFromSource + relativepath + '''"/>'''
    PrintDepth(IndentDepth + 1)
    print '''</Component>'''

    PrintDepth(IndentDepth)    
    print '''</Directory>'''

EncodeDir(root)

print
print
for component in Components:
    print '''<ComponentRef Id=''' + component + ''' />'''



