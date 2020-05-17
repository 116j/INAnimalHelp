package crc6424e3323fbe89298f;


public class MediaService
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("INAnimalHelp.Droid.MediaService, INAnimalHelp.Android", MediaService.class, __md_methods);
	}


	public MediaService ()
	{
		super ();
		if (getClass () == MediaService.class)
			mono.android.TypeManager.Activate ("INAnimalHelp.Droid.MediaService, INAnimalHelp.Android", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
