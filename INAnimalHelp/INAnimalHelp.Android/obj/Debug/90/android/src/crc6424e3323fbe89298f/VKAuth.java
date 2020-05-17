package crc6424e3323fbe89298f;


public class VKAuth
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("INAnimalHelp.Droid.VKAuth, INAnimalHelp.Android", VKAuth.class, __md_methods);
	}


	public VKAuth ()
	{
		super ();
		if (getClass () == VKAuth.class)
			mono.android.TypeManager.Activate ("INAnimalHelp.Droid.VKAuth, INAnimalHelp.Android", "", this, new java.lang.Object[] {  });
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
