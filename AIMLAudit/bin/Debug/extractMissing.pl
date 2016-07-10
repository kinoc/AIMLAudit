$src='normalizedMatchCount4.txt';
open(FIN,"<$src");
%missing=();
%dcoun=();
%rlist=();
while($line = <FIN>)
{
	chomp($line);
	if (length($line)<1) { next;}
	@flds = split(/\t/,$line);
	$input = $flds[0];
	$file =$flds[1];
	$pat =$flds[2];
	$resp =$flds[3];
	$cnt = $flds[4];
	if (length($resp)<2)
	{
	 #print "$line\n";
	 $missing{$input}+=$cnt;
	}
	else
	{
	  $dcount{$input}+=$cnt;
	  if ( index($rlist{$input},$resp) ==-1)
	  {
	    $rlist{$input} .= $resp."|";
	  }
	}
	
}
close(FIN);


open(FOUT,">missing.aiml");
@mkeys = sort {$missing{$b}<=>$missing{$a}} (keys %missing);
print FOUT "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n";
print FOUT "<aiml>\n";
foreach $k (@mkeys)
{
  $uk = uc($k);
  print FOUT "<!-- $uk ($missing{$k}) -->\n";
  print FOUT "<category>\n  <pattern>$uk</pattern>\n  <template></template>\n</cateogry>\n\n";
}
print FOUT "</aiml>\n";
close(FOUT);

open(FOUT,">GT10.aiml");
@mkeys = sort {$dcount{$b}<=>$dcount{$a}} (keys %dcount);
print FOUT "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n";
print FOUT "<aiml>\n";
foreach $k (@mkeys)
{
  if ($dcount{$k}<10) {next;}
  $uk = uc($k);
  @rt = split(/\|/,$rlist{$k});
  $rtext="";
  foreach $r (sort(@rt))
  {
    if (length($r)>1) 
	{
	  $rtext .="     <li>$r</li>\n";
	}
  }
  if (length($rtext)>1)
  {
	  print FOUT "<!-- $uk ($dcount{$k}) -->\n";
	  print FOUT "<category>\n  <pattern>$uk</pattern>\n  <template>\n   <random>\n$rtext   </random>\n  </template>\n</cateogry>\n\n";
  }
}
print FOUT "</aiml>\n";
close(FOUT);