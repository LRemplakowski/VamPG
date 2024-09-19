import sys
import os
from fontTools.ttLib import TTFont

def print_characters(ttf_path):
    try:
        font = TTFont(ttf_path)
        cmap = font['cmap'].getBestCmap()
        font_name = os.path.splitext(os.path.basename(ttf_path))[0]
        output_file = os.path.join(os.path.dirname(ttf_path), f"{font_name}.txt")
        
        with open(output_file, 'w', encoding='utf-8') as f:
            for codepoint in sorted(cmap.keys()):
                f.write(chr(codepoint))
        
        print(f"Characters saved to {output_file}")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python getfontchars.py <path_to_ttf>")
    else:
        print_characters(sys.argv[1])