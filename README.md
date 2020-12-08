# Nota3080bot
Simply checks stock of a few websites. I use for GPUs but can be used for anything from the sellers. FYI this code kinda sucks. This was more an exercise to learn C# than it was to make an effective tool - but it does work even if it is crude and lacking in features. Most of the variables are hardcoded in arrays but you can add whatever GPU models you are actually interested in and it will cycle through all of them. Yeah I understand that is lame and a pain, but I have a 3080 and you don't soo....

Walmart is buggy and I did not spend much time fixing the string match to work better. The others have been fairly reliable. Warning that when something is in stock it will keep sending you emails every 15 seconds so it may get annoying. I realize that can be fixed I just haven't bothered to do it.

Best Buy takes SKU numbers that have to be manually updated.

The others take the URI for the product page.

Microcenter does not sell GPUs or CPUs online so you will probably want to update the "storeSelected" value on line 319 to match your local store.
Dallas is storeSelected=131

Finally, Email info is currently hardcoded. Will fix that shortly in the future. You will need to replace "Put your email here" with your email on lines 487 and 488. Then "put your password here" with, you guessed it, your password. When it is in stock you will get spammed with emails until it is no longer in stock. So like 10 minutes tops. GL. FYI Bestbuy usually comes in stock once a week or so usually between 9 and 10 AM.
